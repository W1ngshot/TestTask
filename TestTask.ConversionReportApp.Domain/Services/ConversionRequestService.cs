using System.Transactions;
using TestTask.ConversionReportApp.Domain.InfrastructureInterfaces;
using TestTask.ConversionReportApp.Domain.ModelExtensions;
using TestTask.ConversionReportApp.Domain.Models;
using TestTask.ConversionReportApp.Domain.Services.Interfaces;

namespace TestTask.ConversionReportApp.Domain.Services;

public class ConversionRequestService(
    IConversionRequestRepository requestRepository,
    IConversionSystem conversionSystem,
    IConversionReportRepository reportRepository,
    IDateTimeProvider dateTimeProvider)
    : IConversionRequestService
{
    public async Task AddRequests(ConversionRequest[] conversionRequests, CancellationToken token)
    {
        await requestRepository.AddRequestsAsync(conversionRequests, token);
    }

    public async Task ProcessImmediateRequests(ProcessImmediateRequest processRequest, CancellationToken token)
    {
        var dateToBeImmediate = dateTimeProvider.OffsetUtcNow - TimeSpan.FromDays(1) +
                                TimeSpan.FromSeconds(processRequest.TimeToBeImmediateInSeconds);

        bool isAnyToProcess;
        do
        {
            isAnyToProcess = await TryProcessRequests(dateToBeImmediate, processRequest.BatchSize, token);
        } while (isAnyToProcess);
    }

    public async Task ProcessOrdinaryRequests(int batchSize, CancellationToken token)
    {
        var dateToBeProcessed = dateTimeProvider.OffsetUtcNow;

        bool isAnyToProcess;
        do
        {
            isAnyToProcess = await TryProcessRequests(dateToBeProcessed, batchSize, token);
        } while (isAnyToProcess);
    }

    private async Task<bool> TryProcessRequests(DateTimeOffset dateToBeProcessed, int batchSize,
        CancellationToken token)
    {
        var requests = (await requestRepository.GetOldestRequestsAsync(batchSize, token)).ToList();

        if (requests.Count == 0 || requests.First().RequestedAt > dateToBeProcessed)
            return false;

        var calculateRequests = requests.ToCalculateRequests();

        var responses = await conversionSystem.CalculateConversionsAsync(calculateRequests, token);

        var reports = responses.Join(requests,
            x => (x.ItemId, x.RegistrationId, x.ConversionDateFrom, x.ConversionDateTo),
            x => (x.ItemId, x.RegistrationId, x.ConversionDateFrom, x.ConversionDateTo),
            (response, request) => new ConversionReport
            {
                ItemId = response.ItemId,
                RegistrationId = response.RegistrationId,
                ConversionDateFrom = response.ConversionDateFrom,
                ConversionDateTo = response.ConversionDateTo,
                ConversionRatio = response.ConversionRatio,
                PaymentsCount = response.PaymentsCount,
                RequestedAt = request.RequestedAt
            });

        using var transaction = CreateTransactionScope();

        await requestRepository.RemoveRequestsAsync(requests, token);
        await reportRepository.AddReportsAsync(reports, token);

        transaction.Complete();
        return true;
    }

    private TransactionScope CreateTransactionScope(
        IsolationLevel level = IsolationLevel.ReadCommitted)
    {
        return new TransactionScope(
            TransactionScopeOption.Required,
            new TransactionOptions
            {
                IsolationLevel = level,
                Timeout = TimeSpan.FromSeconds(10)
            },
            TransactionScopeAsyncFlowOption.Enabled);
    }
}