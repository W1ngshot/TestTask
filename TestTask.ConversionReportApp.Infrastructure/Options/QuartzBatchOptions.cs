namespace TestTask.ConversionReportApp.Infrastructure.Options;

public class QuartzBatchOptions
{
    public const string QuartzBatch = "QuartzBatch";

    public required int SmallBatchSize { get; init; }

    public required int TimeToBeImmediateInSeconds { get; init; }

    public required int LargeBatchSize { get; init; }
}