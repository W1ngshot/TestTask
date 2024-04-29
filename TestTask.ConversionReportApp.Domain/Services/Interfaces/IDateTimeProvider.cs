namespace TestTask.ConversionReportApp.Domain.Services.Interfaces;

public interface IDateTimeProvider
{
    public DateTimeOffset OffsetUtcNow { get; }
}