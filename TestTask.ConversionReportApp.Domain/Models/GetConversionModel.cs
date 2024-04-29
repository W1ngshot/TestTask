namespace TestTask.ConversionReportApp.Domain.Models;

public record GetConversionModel
{
    public required long ItemId { get; init; }

    public required long RegistrationId { get; init; }

    public required PageInfo PageInfo { get; init; }
}