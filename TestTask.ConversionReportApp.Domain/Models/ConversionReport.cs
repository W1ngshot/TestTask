namespace TestTask.ConversionReportApp.Domain.Models;

public class ConversionReport
{
    public required long ItemId { get; init; }
    
    public required long RegistrationId { get; init; }
    
    public required DateTimeOffset ConversionDateFrom { get; init; }
    
    public required DateTimeOffset ConversionDateTo { get; init; }
    
    public required double ConversionRatio { get; init; }

    public required long PaymentsCount { get; init; }
    
    public required DateTimeOffset RequestedAt { get; init; }
}