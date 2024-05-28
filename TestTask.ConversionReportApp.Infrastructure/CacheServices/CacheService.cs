using System.Text.Json;
using Microsoft.Extensions.Caching.Distributed;
using TestTask.ConversionReportApp.Domain.InfrastructureInterfaces;

namespace TestTask.ConversionReportApp.Infrastructure.CacheServices;

public class CacheService<T>(IDistributedCacheWrapper cache) : ICacheService<T>
{
    private readonly DistributedCacheEntryOptions _options = new()
    {
        AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(1)
    };

    public async Task<T?> GetAsync(string key, CancellationToken token)
    {
        var data = await cache.GetStringAsync(key, token);

        return data is null ? default : JsonSerializer.Deserialize<T>(data);
    }

    public async Task SetAsync(string key, T item, CancellationToken token)
    {
        var data = JsonSerializer.Serialize(item);
        await cache.SetStringAsync(key, data, _options, token);
    }
}