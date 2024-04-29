namespace TestTask.ConversionReportApp.Infrastructure.Entities;

public class ConversionRequestEntityV1
{
    public long ItemId { get; init; }
    
    public long RegistrationId { get; init; }
    
    public DateTimeOffset RequestFrom { get; init; }
    
    public DateTimeOffset RequestTo { get; init; }
    
    public DateTimeOffset RequestedAt { get; init; }
}