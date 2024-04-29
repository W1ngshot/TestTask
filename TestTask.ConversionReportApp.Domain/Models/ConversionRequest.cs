namespace TestTask.ConversionReportApp.Domain.Models;

public class ConversionRequest
{
    public long ItemId { get; init; }
    
    public long RegistrationId { get; init; }
    
    public DateTimeOffset ConversionDateFrom { get; init; }
    
    public DateTimeOffset ConversionDateTo { get; init; }
    
    public DateTimeOffset RequestedAt { get; init; }
}