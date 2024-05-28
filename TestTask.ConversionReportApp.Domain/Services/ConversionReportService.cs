using TestTask.ConversionReportApp.Domain.Common;
using TestTask.ConversionReportApp.Domain.InfrastructureInterfaces;
using TestTask.ConversionReportApp.Domain.Models;
using TestTask.ConversionReportApp.Domain.Services.Interfaces;

namespace TestTask.ConversionReportApp.Domain.Services;

public class ConversionReportService(
    IConversionReportRepository reportRepository,
    ICacheService<IEnumerable<ConversionReport>> cacheService) : IConversionReportService
{
    public async Task<IEnumerable<ConversionReport>> GetReportsAsync(GetConversionModel model,
        CancellationToken token)
    {
        var key = CacheKeysGenerator.GetReportsKey(model);
        var cachedReports = await cacheService.GetAsync(key, token);
        if (cachedReports is not null)
            return cachedReports;

        var request = new GetConversionRequest
        {
            ItemId = model.ItemId,
            RegistrationId = model.RegistrationId,
            Take = model.PageInfo.ElementsPerPage,
            Skip = (model.PageInfo.PageNumber - 1) * model.PageInfo.ElementsPerPage
        };
        var reports = (await reportRepository.GetReportsAsync(request, token)).ToList();

        await cacheService.SetAsync(key, reports, token);
        return reports;
    }
}