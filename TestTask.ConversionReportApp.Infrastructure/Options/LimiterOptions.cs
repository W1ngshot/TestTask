namespace TestTask.ConversionReportApp.Infrastructure.Options;

public class LimiterOptions
{
    public const string RateLimiter = "RateLimiter";

    public required string Name { get; init; }

    public required int WindowInSeconds { get; init; }

    public required int PermitLimit { get; init; }
}