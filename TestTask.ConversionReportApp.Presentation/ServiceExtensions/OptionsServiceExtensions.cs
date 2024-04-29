using TestTask.ConversionReportApp.Infrastructure.Options;

namespace TestTask.ConversionReportApp.Presentation.ServiceExtensions;

public static class OptionsServiceExtensions
{
    public static IServiceCollection AddConfiguredOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<PostgresOptions>(configuration.GetSection(PostgresOptions.Postgres));
        services.Configure<KafkaOptions>(configuration.GetSection(KafkaOptions.Kafka));
        services.Configure<QuartzBatchOptions>(configuration.GetSection(QuartzBatchOptions.QuartzBatch));
        services.Configure<RedisOptions>(configuration.GetSection(RedisOptions.Redis));
        services.Configure<LimiterOptions>(configuration.GetSection(LimiterOptions.RateLimiter));
        
        return services;
    }
}