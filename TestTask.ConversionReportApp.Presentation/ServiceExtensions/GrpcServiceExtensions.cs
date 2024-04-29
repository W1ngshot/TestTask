using TestTask.ConversionReportApp.Infrastructure.Options;
using TestTask.ConversionReportApp.Presentation.Interceptors;
using TestTask.ConversionReportApp.Presentation.Services;

namespace TestTask.ConversionReportApp.Presentation.ServiceExtensions;

public static class GrpcServiceExtensions
{
    public static IServiceCollection AddGrpcConfiguration(this IServiceCollection services)
    {
        services.AddGrpc(options =>
        {
            options.Interceptors.Add<ValidationInterceptor>();
            options.Interceptors.Add<ExceptionHandlingInterceptor>();
        }).AddJsonTranscoding();

        return services;
    }
    
    public static void MapGrpcServices(this IEndpointRouteBuilder app, IConfiguration configuration)
    {
        var limiterOptions = configuration.GetRequiredSection(LimiterOptions.RateLimiter)
            .Get<LimiterOptions>() ?? throw new ArgumentNullException(nameof(LimiterOptions));
        
        app.MapGrpcService<ReportApiService>().RequireRateLimiting(limiterOptions.Name);
    }
}