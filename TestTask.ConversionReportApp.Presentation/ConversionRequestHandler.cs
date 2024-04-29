using System.Runtime.Serialization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Confluent.Kafka;
using TestTask.ConversionReportApp.Domain.Services.Interfaces;
using TestTask.ConversionReportApp.Infrastructure.Kafka;
using TestTask.ConversionReportApp.Presentation.ModelExtensions;
using TestTask.ConversionReportApp.Presentation.Models;

namespace TestTask.ConversionReportApp.Presentation;

public class ConversionRequestHandler(
    ILogger<ConversionRequestHandler> logger,
    IConversionRequestService conversionRequestService)
    : IHandler<Ignore, string>
{
    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        Converters = {new JsonStringEnumConverter()}
    };

    public async Task Handle(IReadOnlyCollection<ConsumeResult<Ignore, string>> messages, CancellationToken token)
    {
        await Task.Delay(Random.Shared.Next(300), token);

        var requestEvents = messages.Select(message =>
                JsonSerializer.Deserialize<ConversionRequestEvent>(message.Message.Value, _jsonSerializerOptions) ??
                throw new SerializationException($"{nameof(ConversionRequestEvent)}: {message.Message.Value}"))
            .ToList();

        await conversionRequestService.AddRequests(requestEvents.ToDomainRequests().ToArray(), token);

        logger.LogInformation("Handled {Count} messages", messages.Count);
    }
}