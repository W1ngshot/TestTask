using TestTask.ConversionReportApp.Domain.InfrastructureInterfaces;
using TestTask.ConversionReportApp.Domain.Models;
using TestTask.ConversionReportApp.Domain.Services.Interfaces;

namespace TestTask.ConversionReportApp.Domain.Services;

public class ConversionReportService(
    IConversionReportRepository reportRepository,
    IReportCacheService cacheService) : IConversionReportService
{
    public async Task<IEnumerable<ConversionReport>> GetReportsAsync(GetConversionModel model,
        CancellationToken token)
    {
        var cachedReports = await cacheService.GetAsync(model, token);
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

        await cacheService.SetAsync(model, reports, token);
        return reports;
    }
}