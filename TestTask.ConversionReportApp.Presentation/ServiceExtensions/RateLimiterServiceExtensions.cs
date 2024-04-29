using Microsoft.AspNetCore.RateLimiting;
using TestTask.ConversionReportApp.Infrastructure.Options;

namespace TestTask.ConversionReportApp.Presentation.ServiceExtensions;

public static class RateLimiterServiceExtensions
{
    public static IServiceCollection AddRateLimiterConfiguration(this IServiceCollection services,
        IConfiguration configuration)
    {
        var limiterOptions = configuration.GetRequiredSection(LimiterOptions.RateLimiter)
            .Get<LimiterOptions>() ?? throw new ArgumentNullException(nameof(LimiterOptions));
        
        return services.AddRateLimiter(options =>
        {
            options.AddSlidingWindowLimiter(limiterOptions.Name, opt =>
            {
                opt.Window = TimeSpan.FromSeconds(limiterOptions.WindowInSeconds);
                opt.PermitLimit = limiterOptions.PermitLimit;
                opt.SegmentsPerWindow = 1;
            });
        });
    }
}