namespace TestTask.ConversionReportApp.Infrastructure.Entities;

public record ConversionReportEntityV1
{
    public long ItemId { get; init; }
    
    public long RegistrationId { get; init; }
    
    public DateTimeOffset RequestFrom { get; init; }
    
    public DateTimeOffset RequestTo { get; init; }
    
    public double ConversionRatio { get; init; }

    public long PaymentsCount { get; init; }
    
    public DateTimeOffset RequestedAt { get; init; }
}