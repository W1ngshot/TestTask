using Microsoft.Extensions.Caching.Distributed;

namespace TestTask.ConversionReportApp.Infrastructure.CacheServices;

public class DistributedCacheWrapper(IDistributedCache cache) : IDistributedCacheWrapper
{
    public async Task SetStringAsync(string key, string data, DistributedCacheEntryOptions options,
        CancellationToken token)
    {
        await cache.SetStringAsync(key, data, token);
    }

    public async Task SetStringAsync(string key, string data, CancellationToken token)
    {
        await cache.SetStringAsync(key, data, token);
    }

    public async Task<string?> GetStringAsync(string key, CancellationToken token)
    {
        return await cache.GetStringAsync(key, token);
    }
}