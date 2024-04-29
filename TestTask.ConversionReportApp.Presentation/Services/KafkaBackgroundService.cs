using Confluent.Kafka;
using Microsoft.Extensions.Options;
using TestTask.ConversionReportApp.Infrastructure.Kafka;
using TestTask.ConversionReportApp.Infrastructure.Options;

namespace TestTask.ConversionReportApp.Presentation.Services;

public class KafkaBackgroundService(
    IServiceProvider serviceProvider,
    ILogger<KafkaBackgroundService> logger,
    IHandler<Ignore, string> handler)
    : BackgroundService
{
    private readonly KafkaAsyncConsumer<Ignore, string> _consumer = new(
        handler,
        null,
        null,
        serviceProvider.GetRequiredService<ILogger<KafkaAsyncConsumer<Ignore, string>>>(),
        serviceProvider.GetRequiredService<IOptions<KafkaOptions>>());

    public override Task StopAsync(CancellationToken cancellationToken)
    {
        _consumer.Dispose();

        return Task.CompletedTask;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            await _consumer.Consume(stoppingToken);
            _consumer.Dispose();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unhandled exception occured");
        }
    }
}