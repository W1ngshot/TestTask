using Quartz;
using TestTask.ConversionReportApp.Presentation.Jobs;

namespace TestTask.ConversionReportApp.Presentation.ServiceExtensions;

public static class QuartzServiceExtensions
{
    public static IServiceCollection AddQuartzConfiguration(this IServiceCollection services)
    {
        services.AddQuartz(q =>
        {
            q.UseDefaultThreadPool(tp => tp.MaxConcurrency = 1);

            q.ScheduleJob<HourlyRequestHandlingJob>(trigger => trigger
                .WithIdentity("Hourly job trigger")
                .StartNow()
                .WithDailyTimeIntervalSchedule(x => x.WithInterval(1, IntervalUnit.Hour))
                .WithDescription("Hourly trigger for immediate requests with small batch size")
            );

            q.ScheduleJob<DailyRequestHandlingJob>(trigger => trigger
                .WithIdentity("Night job trigger")
                .StartNow()
                .WithCronSchedule("0 0 20 1/1 * ? *", x => x.InTimeZone(TimeZoneInfo.Utc))
                .WithDescription("Night trigger for ordinary requests with large batch size")
            );
        });

        services.AddTransient<HourlyRequestHandlingJob>();
        services.AddTransient<DailyRequestHandlingJob>();

        services.AddQuartzHostedService(options => options.WaitForJobsToComplete = true);

        return services;
    }
}