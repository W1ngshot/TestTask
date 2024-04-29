using System.Transactions;
using Microsoft.Extensions.Logging;
using Npgsql;
using Polly;
using Polly.Contrib.WaitAndRetry;

namespace TestTask.ConversionReportApp.Infrastructure.Repositories;

public abstract class PgRepository(NpgsqlDataSource dataSource, ILogger<PgRepository> logger)
{
    protected const int DefaultTimeoutInSeconds = 5;
    private const int RetriesCount = 5;

    protected readonly AsyncPolicy RetryPolicy = Policy
        .Handle<Exception>()
        .WaitAndRetryAsync(
            Backoff.DecorrelatedJitterBackoffV2(
                medianFirstRetryDelay: TimeSpan.FromMilliseconds(50),
                retryCount: RetriesCount,
                fastFirst: false),
            onRetry: (exception, timespan, retryAttempt, _) =>
                logger.LogError(
                    "{delay} ms, then making retry {retry}. Exception: {message}",
                    timespan.TotalMilliseconds, retryAttempt, exception.Message));

    protected async Task<NpgsqlConnection> GetConnection()
    {
        if (Transaction.Current is not null &&
            Transaction.Current.TransactionInformation.Status is TransactionStatus.Aborted)
        {
            throw new TransactionAbortedException("Transaction was aborted (probably by user cancellation request)");
        }

        var connection = await dataSource.OpenConnectionAsync();

        await connection.ReloadTypesAsync();

        return connection;
    }
}