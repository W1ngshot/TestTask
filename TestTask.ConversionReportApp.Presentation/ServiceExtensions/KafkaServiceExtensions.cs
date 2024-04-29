using TestTask.ConversionReportApp.Infrastructure.Kafka;
using TestTask.ConversionReportApp.Presentation.Services;

namespace TestTask.ConversionReportApp.Presentation.ServiceExtensions;

public static class KafkaServiceExtensions
{
    public static IServiceCollection AddKafkaHandler<TKey, TValue, THandler>(this IServiceCollection services)
        where THandler : class, IHandler<TKey, TValue>
    {
        services.AddSingleton<IHandler<TKey, TValue>, THandler>();
        
        return services;
    }

    public static IServiceCollection AddKafkaBackgroundService(this IServiceCollection services)
    { 
        services.AddHostedService<KafkaBackgroundService>();
        
        return services;
    }
}