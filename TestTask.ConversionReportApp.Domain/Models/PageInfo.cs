namespace TestTask.ConversionReportApp.Domain.Models;

public record PageInfo
{
    public required int PageNumber { get; init; }
    
    public required int ElementsPerPage { get; init; }
}