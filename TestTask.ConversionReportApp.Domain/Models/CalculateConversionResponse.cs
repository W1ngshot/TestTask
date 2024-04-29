namespace TestTask.ConversionReportApp.Domain.Models;

public class CalculateConversionResponse
{
    public long ItemId { get; init; }
    
    public long RegistrationId { get; init; }
    
    public DateTimeOffset ConversionDateFrom { get; init; }
    
    public DateTimeOffset ConversionDateTo { get; init; }
    
    public double ConversionRatio { get; init; }

    public long PaymentsCount { get; init; }
}