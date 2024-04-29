using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using TestTask.ConversionReportApp.Domain.InfrastructureInterfaces;
using TestTask.ConversionReportApp.Domain.Models;

namespace TestTask.ConversionReportApp.Infrastructure.CacheServices;

public class ReportCacheService(IDistributedCache cache) : RedisCacheService, IReportCacheService
{
    protected override string KeyPrefix => "reports";

    private readonly DistributedCacheEntryOptions _options = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1)
    };

    public async Task<IEnumerable<ConversionReport>?> GetAsync(GetConversionModel model, CancellationToken token)
    {
        var key = GetKey(
            "item_id", model.ItemId,
            "registration_id", model.RegistrationId,
            model.PageInfo.PageNumber, model.PageInfo.ElementsPerPage);

        var data = await cache.GetStringAsync(key!, token);

        return data is null ? null : JsonSerializer.Deserialize<IEnumerable<ConversionReport>>(data);
    }

    public async Task SetAsync(GetConversionModel model, IEnumerable<ConversionReport> reports, CancellationToken token)
    {
        var key = GetKey(
            "item_id", model.ItemId,
            "registration_id", model.RegistrationId,
            model.PageInfo.PageNumber, model.PageInfo.ElementsPerPage);

        var data = JsonSerializer.Serialize(reports);

        await cache.SetStringAsync(key!, data, _options, token);
    }
}