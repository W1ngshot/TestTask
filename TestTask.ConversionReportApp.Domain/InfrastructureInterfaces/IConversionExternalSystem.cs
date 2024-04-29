using TestTask.ConversionReportApp.Domain.Models;

namespace TestTask.ConversionReportApp.Domain.InfrastructureInterfaces;

public interface IConversionSystem
{
    public Task<IEnumerable<CalculateConversionResponse>> CalculateConversionsAsync(
        IEnumerable<CalculateConversionRequest> requests,
        CancellationToken token);
}