using TestTask.ConversionReportApp.Domain.Services;
using TestTask.ConversionReportApp.Domain.Services.Interfaces;

namespace TestTask.ConversionReportApp.Presentation.ServiceExtensions;

public static class DomainServicesExtensions
{
    public static IServiceCollection AddDomainServices(this IServiceCollection services)
    {
        services.AddSingleton<IConversionRequestService, ConversionRequestService>();
        services.AddSingleton<IConversionReportService, ConversionReportService>();
        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        return services;
    }
}