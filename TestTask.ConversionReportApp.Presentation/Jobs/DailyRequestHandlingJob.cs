using Microsoft.Extensions.Options;
using Quartz;
using TestTask.ConversionReportApp.Domain.Services.Interfaces;
using TestTask.ConversionReportApp.Infrastructure.Options;

namespace TestTask.ConversionReportApp.Presentation.Jobs;

public class DailyRequestHandlingJob(
    IOptions<QuartzBatchOptions> options,
    IConversionRequestService requestService,
    ILogger<DailyRequestHandlingJob> logger) : IJob
{
    public async Task Execute(IJobExecutionContext context)
    {
        try
        {
            var batchSize = options.Value.LargeBatchSize;
            await requestService.ProcessOrdinaryRequests(batchSize, context.CancellationToken);
            logger.LogInformation("Daily job executed with {size} batch size", batchSize);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Daily job failed. Have to retry it manually.");
            throw;
        }
    }
}