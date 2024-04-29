using TestTask.ConversionReportApp.Domain.Models;

namespace TestTask.ConversionReportApp.Domain.InfrastructureInterfaces;

public interface IReportCacheService
{
    public Task<IEnumerable<ConversionReport>?> GetAsync(GetConversionModel model, CancellationToken token);

    public Task SetAsync(GetConversionModel model, IEnumerable<ConversionReport> reports, CancellationToken token);
}