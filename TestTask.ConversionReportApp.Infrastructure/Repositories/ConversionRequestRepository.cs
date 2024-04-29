using Dapper;
using Microsoft.Extensions.Logging;
using Npgsql;
using TestTask.ConversionReportApp.Domain.InfrastructureInterfaces;
using TestTask.ConversionReportApp.Domain.Models;
using TestTask.ConversionReportApp.Infrastructure.Entities;
using TestTask.ConversionReportApp.Infrastructure.EntityExtensions;

namespace TestTask.ConversionReportApp.Infrastructure.Repositories;

public class ConversionRequestRepository(NpgsqlDataSource dataSource, ILogger<PgRepository> logger)
    : PgRepository(dataSource, logger), IConversionRequestRepository
{
    public async Task AddRequestsAsync(IEnumerable<ConversionRequest> requests, CancellationToken token)
    {
        const string sqlQuery = @"
insert into conversion_requests (item_id, registration_id, request_from, request_to, requested_at)  
select item_id, registration_id, request_from, request_to, requested_at
  from UNNEST(@Requests);
";

        await RetryPolicy.ExecuteAsync(async () =>
        {
            await using var connection = await GetConnection();
            await connection.ExecuteAsync(
                new CommandDefinition(
                    sqlQuery,
                    new
                    {
                        Requests = requests.ToRequestEntities()
                    },
                    cancellationToken: token));
        });
    }

    public async Task<IEnumerable<ConversionRequest>> GetOldestRequestsAsync(int limit, CancellationToken token)
    {
        var sqlQuery = @"
select item_id
     , registration_id
     , request_from
     , request_to
     , requested_at
  from conversion_requests
 order by requested_at
 limit @Limit
";
        return await RetryPolicy.ExecuteAsync(async () =>
        {
            await using var connection = await GetConnection();
            var entities = await connection.QueryAsync<ConversionRequestEntityV1>(
                new CommandDefinition(
                    sqlQuery,
                    new
                    {
                        Limit = limit
                    },
                    cancellationToken: token));
            return entities.ToDomainRequests();
        });
    }

    public async Task RemoveRequestsAsync(IEnumerable<ConversionRequest> requests, CancellationToken token)
    {
        const string sqlQuery = @"
delete from conversion_requests
 where (item_id, registration_id, request_from, request_to) in 
            (select item_id, registration_id, request_from, request_to 
               from UNNEST(@Requests));
";

        await RetryPolicy.ExecuteAsync(async () =>
        {
            await using var connection = await GetConnection();
            await connection.ExecuteAsync(
                new CommandDefinition(
                    sqlQuery,
                    new
                    {
                        Requests = requests.ToRequestEntities()
                    },
                    cancellationToken: token));
        });
    }
}