using Dapper;
using Microsoft.Extensions.Logging;
using Npgsql;
using TestTask.ConversionReportApp.Domain.InfrastructureInterfaces;
using TestTask.ConversionReportApp.Domain.Models;
using TestTask.ConversionReportApp.Infrastructure.Entities;
using TestTask.ConversionReportApp.Infrastructure.EntityExtensions;

namespace TestTask.ConversionReportApp.Infrastructure.Repositories;

public class ConversionReportRepository(NpgsqlDataSource dataSource, ILogger<PgRepository> logger)
    : PgRepository(dataSource, logger), IConversionReportRepository
{
    public async Task AddReportsAsync(IEnumerable<ConversionReport> reports, CancellationToken token)
    {
        const string sqlQuery = @"
insert into conversion_reports (item_id, registration_id, request_from, request_to, conversion_ratio, payments_count, requested_at)  
select item_id, registration_id, request_from, request_to, conversion_ratio, payments_count, requested_at
  from UNNEST(@Reports);
";

        await RetryPolicy.ExecuteAsync(async () =>
        {
            await using var connection = await GetConnection();
            await connection.ExecuteAsync(
                new CommandDefinition(
                    sqlQuery,
                    new
                    {
                        Reports = reports.ToReportEntities()
                    },
                    cancellationToken: token));
        });
    }

    public async Task<IEnumerable<ConversionReport>> GetReportsAsync(GetConversionRequest request,
        CancellationToken token)
    {
        var sqlQuery = @"
select item_id
     , registration_id
     , request_from
     , request_to
     , conversion_ratio
     , payments_count
     , requested_at
  from conversion_reports
 where item_id = @ItemId and registration_id = @RegistrationId
 order by requested_at desc
offset @Offset
 limit @Limit
";

        return await RetryPolicy.ExecuteAsync(async () =>
        {
            await using var connection = await GetConnection();
            var entities = await connection.QueryAsync<ConversionReportEntityV1>(
                new CommandDefinition(
                    sqlQuery,
                    new
                    {
                        ItemId = request.ItemId,
                        RegistrationId = request.RegistrationId,
                        Offset = request.Skip,
                        Limit = request.Take
                    },
                    cancellationToken: token));

            return entities.ToDomainReports();
        });
    }
}