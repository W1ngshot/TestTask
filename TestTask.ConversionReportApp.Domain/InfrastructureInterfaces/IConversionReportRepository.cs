using TestTask.ConversionReportApp.Domain.Models;

namespace TestTask.ConversionReportApp.Domain.InfrastructureInterfaces;

public interface IConversionReportRepository
{
    public Task AddReportsAsync(IEnumerable<ConversionReport> reports, CancellationToken token);

    public Task<IEnumerable<ConversionReport>> GetReportsAsync(GetConversionRequest request, CancellationToken token);
}