using Microsoft.Extensions.Options;
using Quartz;
using TestTask.ConversionReportApp.Domain.Models;
using TestTask.ConversionReportApp.Domain.Services.Interfaces;
using TestTask.ConversionReportApp.Infrastructure.Options;

namespace TestTask.ConversionReportApp.Presentation.Jobs;

public class HourlyRequestHandlingJob(
    IOptions<QuartzBatchOptions> options,
    IConversionRequestService requestService,
    ILogger<HourlyRequestHandlingJob> logger) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            var request = new ProcessImmediateRequest
            {
                BatchSize = options.Value.SmallBatchSize,
                TimeToBeImmediateInSeconds = options.Value.TimeToBeImmediateInSeconds
            };
            await requestService.ProcessImmediateRequests(request, context.CancellationToken);
            logger.LogInformation("Hourly job executed with {size} batch size", request.BatchSize);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Hourly job failed. Have to retry it manually.");
            throw;
        }
    }
}