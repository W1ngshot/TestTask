namespace TestTask.ConversionReportApp.Infrastructure.Options;

public class RedisOptions
{
    public const string Redis = "Redis";
    
    public required string ConnectionString { get; init; }
}