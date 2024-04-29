using FluentMigrator.Runner;
using Microsoft.Extensions.Options;
using Npgsql;
using Npgsql.NameTranslation;
using TestTask.ConversionReportApp.Domain.InfrastructureInterfaces;
using TestTask.ConversionReportApp.Infrastructure.Entities;
using TestTask.ConversionReportApp.Infrastructure.ExternalServices;
using TestTask.ConversionReportApp.Infrastructure.Options;
using TestTask.ConversionReportApp.Infrastructure.Repositories;

namespace TestTask.ConversionReportApp.Presentation.ServiceExtensions;

public static class PostgresConfigurationServiceExtensions
{
    private static readonly INpgsqlNameTranslator Translator = new NpgsqlSnakeCaseNameTranslator();

    public static IServiceCollection AddPostgresConfiguration(this IServiceCollection services,
        IConfiguration configuration)
    {
        Dapper.DefaultTypeMap.MatchNamesWithUnderscores = true;

        var postgresOptions = configuration.GetRequiredSection(PostgresOptions.Postgres)
            .Get<PostgresOptions>() ?? throw new ArgumentNullException(nameof(PostgresOptions));

        services.AddNpgsqlDataSource(
            postgresOptions.ConnectionString,
            builder => builder
                .MapComposite<ConversionReportEntityV1>("report_v1", Translator)
                .MapComposite<ConversionRequestEntityV1>("request_v1", Translator));

        return services;
    }

    public static IServiceCollection AddMigrations(this IServiceCollection services)
    {
        return services.AddFluentMigratorCore()
            .ConfigureRunner(rb => rb.AddPostgres()
                .WithGlobalConnectionString(provider =>
                {
                    var cfg = provider.GetRequiredService<IOptions<PostgresOptions>>();
                    return cfg.Value.ConnectionString;
                })
                .ScanIn(typeof(PostgresOptions).Assembly).For.Migrations()
            )
            .AddLogging(lb => lb.AddFluentMigratorConsole());
    }

    public static IServiceCollection AddPostgresRepositories(this IServiceCollection services)
    {
        services.AddSingleton<IConversionReportRepository, ConversionReportRepository>();
        services.AddSingleton<IConversionRequestRepository, ConversionRequestRepository>();

        return services;
    }

    public static IServiceCollection AddExternalServices(this IServiceCollection services)
    {
        services.AddSingleton<IConversionSystem, FakeConversionSystem>();

        return services;
    }
}