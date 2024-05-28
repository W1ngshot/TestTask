using Microsoft.Extensions.Caching.Distributed;

namespace TestTask.ConversionReportApp.Infrastructure.CacheServices;

public interface IDistributedCacheWrapper
{
    Task SetStringAsync(string key, string data, CancellationToken token);
    
    public Task SetStringAsync(string key, string data, DistributedCacheEntryOptions options, CancellationToken token);

    Task<string?> GetStringAsync(string key, CancellationToken token);
}