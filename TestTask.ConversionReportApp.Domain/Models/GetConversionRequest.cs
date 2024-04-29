namespace TestTask.ConversionReportApp.Domain.Models;

public class GetConversionRequest
{
    public required long ItemId { get; init; }
    
    public required long RegistrationId { get; init; }
    
    public required int Skip { get; init; }
    
    public required int Take { get; init; }
}