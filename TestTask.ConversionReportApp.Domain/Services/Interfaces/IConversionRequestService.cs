using TestTask.ConversionReportApp.Domain.Models;

namespace TestTask.ConversionReportApp.Domain.Services.Interfaces;

public interface IConversionRequestService
{
    public Task AddRequests(ConversionRequest[] conversionRequests, CancellationToken token);

    public Task ProcessImmediateRequests(ProcessImmediateRequest processRequest, CancellationToken token);

    public Task ProcessOrdinaryRequests(int batchSize, CancellationToken token);
}