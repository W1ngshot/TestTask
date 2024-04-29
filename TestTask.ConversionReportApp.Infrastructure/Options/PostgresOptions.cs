namespace TestTask.ConversionReportApp.Infrastructure.Options;

public record PostgresOptions
{
    public const string Postgres = "Postgres";
    
    public required string ConnectionString { get; init; }
}