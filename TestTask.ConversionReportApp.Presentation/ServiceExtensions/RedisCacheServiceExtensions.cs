using TestTask.ConversionReportApp.Domain.InfrastructureInterfaces;
using TestTask.ConversionReportApp.Infrastructure.CacheServices;
using TestTask.ConversionReportApp.Infrastructure.Options;

namespace TestTask.ConversionReportApp.Presentation.ServiceExtensions;

public static class RedisCacheServiceExtensions
{
    public static IServiceCollection AddRedisCache(this IServiceCollection services, IConfiguration configuration)
    {
        var redisOptions = configuration.GetRequiredSection(RedisOptions.Redis)
            .Get<RedisOptions>() ?? throw new ArgumentNullException(nameof(RedisOptions));
        
        services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redisOptions.ConnectionString;
        });

        services.AddSingleton<IDistributedCacheWrapper, DistributedCacheWrapper>();
        
        return services;
    }
    
    public static IServiceCollection AddRedisCacheServices(this IServiceCollection services)
    {
        services.AddSingleton(typeof(ICacheService<>), typeof(CacheService<>));
        
        return services;
    }
}