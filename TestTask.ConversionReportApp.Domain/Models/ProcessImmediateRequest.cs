namespace TestTask.ConversionReportApp.Domain.Models;

public record ProcessImmediateRequest
{
    public required int BatchSize { get; init; }

    public required int TimeToBeImmediateInSeconds { get; init; }
}