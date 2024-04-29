using System.Threading.Channels;
using Confluent.Kafka;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Polly;
using Polly.Retry;
using TestTask.ConversionReportApp.Domain.Common;
using TestTask.ConversionReportApp.Infrastructure.Options;

namespace TestTask.ConversionReportApp.Infrastructure.Kafka;

public sealed class KafkaAsyncConsumer<TKey, TValue> : IDisposable
{
    private readonly Channel<ConsumeResult<TKey, TValue>> _channel;
    private readonly IConsumer<TKey, TValue> _consumer;
    private readonly IHandler<TKey, TValue> _handler;
    private readonly ILogger<KafkaAsyncConsumer<TKey, TValue>> _logger;
    private readonly AsyncRetryPolicy _retryPolicy;
    private readonly KafkaOptions _kafkaOptions;

    public KafkaAsyncConsumer(
        IHandler<TKey, TValue> handler,
        IDeserializer<TKey>? keyDeserializer,
        IDeserializer<TValue>? valueDeserializer,
        ILogger<KafkaAsyncConsumer<TKey, TValue>> logger,
        IOptions<KafkaOptions> kafkaOptions)
    {
        var builder = new ConsumerBuilder<TKey, TValue>(
            new ConsumerConfig
            {
                BootstrapServers = kafkaOptions.Value.BootstrapServers,
                GroupId = kafkaOptions.Value.GroupId,
                AutoOffsetReset = AutoOffsetReset.Earliest,
                EnableAutoCommit = true,
                EnableAutoOffsetStore = false
            });

        if (keyDeserializer is not null)
        {
            builder.SetKeyDeserializer(keyDeserializer);
        }

        if (valueDeserializer is not null)
        {
            builder.SetValueDeserializer(valueDeserializer);
        }

        _handler = handler;
        _logger = logger;

        _retryPolicy = Policy
            .Handle<Exception>()
            .WaitAndRetryForeverAsync(retry => TimeSpan.FromMilliseconds(
                    Random.Shared.Next(200 * retry, 200 * (retry + 1))),
                onRetry: (exception, waitDuration) =>
                    _logger.LogError($"Failed. Waited {waitDuration} seconds. {exception.Message} {exception}"));
            // .WaitAndRetryAsync(
            //     Backoff.DecorrelatedJitterBackoffV2(
            //         medianFirstRetryDelay: TimeSpan.FromMilliseconds(20),
            //         retryCount: options.RetryCount,
            //         fastFirst: false),
            //     onRetry: (_, timespan, retryAttempt, _) =>
            //         logger.LogError(
            //             "{delay} ms, then making retry {retry}.",
            //             timespan.TotalMilliseconds, retryAttempt));
            // DO THIS WITH DLQ

        _kafkaOptions = kafkaOptions.Value;

        _channel = Channel.CreateBounded<ConsumeResult<TKey, TValue>>(
            new BoundedChannelOptions(kafkaOptions.Value.BatchSize)
            {
                SingleWriter = true,
                SingleReader = true,
                AllowSynchronousContinuations = true,
                FullMode = BoundedChannelFullMode.Wait
            });

        _consumer = builder.Build();
        _consumer.Subscribe(kafkaOptions.Value.Topic);
        Thread.Sleep(10000);
    }

    public Task Consume(CancellationToken token)
    {
        var handle = HandleCore(token);
        var consume = ConsumeCore(token);

        return Task.WhenAll(handle, consume);
    }

    private async Task HandleCore(CancellationToken token)
    {
        await Task.Yield();

        await foreach (var consumeResults in _channel.Reader
                           .ReadAllAsync(token)
                           .Buffer(_kafkaOptions.BatchSize, TimeSpan.FromSeconds(_kafkaOptions.BatchDelayInSeconds))
                           .WithCancellation(token))
        {
            token.ThrowIfCancellationRequested();

            await _retryPolicy.ExecuteAsync(async () => { await _handler.Handle(consumeResults, token); });

            var partitionLastOffsets = consumeResults
                .GroupBy(
                    r => r.Partition.Value,
                    (_, f) => f.MaxBy(p => p.Offset.Value));

            foreach (var partitionLastOffset in partitionLastOffsets)
            {
                _consumer.StoreOffset(partitionLastOffset);
            }
        }
    }

    private async Task ConsumeCore(CancellationToken token)
    {
        await Task.Yield();

        while (_consumer.Consume(token) is { } result)
        {
            await _channel.Writer.WriteAsync(result, token);
            _logger.LogTrace(
                "{Partition}:{Offset}:WriteToChannel",
                result.Partition.Value,
                result.Offset.Value);
        }

        _channel.Writer.Complete();
    }

    public void Dispose() => _consumer.Close();
}