using FluentValidation;

namespace TestTask.ConversionReportApp.Presentation.ServiceExtensions;

public static class ValidationServiceExtensions
{
    public static IServiceCollection AddFluentValidation(this IServiceCollection services)
    {
        services.AddValidatorsFromAssemblyContaining<Program>();
        return services;
    }
}