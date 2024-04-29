namespace TestTask.ConversionReportApp.Infrastructure.Options;

public record KafkaOptions
{
    public const string Kafka = "Kafka";

    public required string BootstrapServers { get; init; }

    public required string GroupId { get; init; }

    public required string Topic { get; init; }

    public required int BatchSize { get; init; }

    public required int BatchDelayInSeconds { get; init; }
}