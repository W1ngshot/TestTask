using TestTask.ConversionReportApp.Domain.Services.Interfaces;

namespace TestTask.ConversionReportApp.Domain.Services;

public class DateTimeProvider : IDateTimeProvider
{
    public DateTimeOffset OffsetUtcNow => DateTimeOffset.UtcNow;
}