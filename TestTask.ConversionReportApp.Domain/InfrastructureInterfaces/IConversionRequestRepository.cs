using TestTask.ConversionReportApp.Domain.Models;

namespace TestTask.ConversionReportApp.Domain.InfrastructureInterfaces;

public interface IConversionRequestRepository
{
    public Task AddRequestsAsync(IEnumerable<ConversionRequest> requests, CancellationToken token);

    public Task<IEnumerable<ConversionRequest>> GetOldestRequestsAsync(int limit, CancellationToken token);

    public Task RemoveRequestsAsync(IEnumerable<ConversionRequest> requests, CancellationToken token);
}