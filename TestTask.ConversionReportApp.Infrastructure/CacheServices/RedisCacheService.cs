using StackExchange.Redis;

namespace TestTask.ConversionReportApp.Infrastructure.CacheServices;

public abstract class RedisCacheService
{
    protected abstract string KeyPrefix { get; }
    
    protected RedisKey GetKey(params object[] identifiers)
        => new ($"{KeyPrefix}:{string.Join(':', identifiers)}");
}