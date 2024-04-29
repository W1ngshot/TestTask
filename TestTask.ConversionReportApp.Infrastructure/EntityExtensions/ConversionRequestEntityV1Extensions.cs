using TestTask.ConversionReportApp.Domain.Common;
using TestTask.ConversionReportApp.Domain.Models;
using TestTask.ConversionReportApp.Infrastructure.Entities;

namespace TestTask.ConversionReportApp.Infrastructure.EntityExtensions;

public static class ConversionRequestEntityV1Extensions
{
    private static ConversionRequestEntityV1 ToRequestEntity(this ConversionRequest request)
    {
        return new ConversionRequestEntityV1
        {
            ItemId = request.ItemId,
            RegistrationId = request.RegistrationId,
            RequestFrom = request.ConversionDateFrom,
            RequestTo = request.ConversionDateTo,
            RequestedAt = request.RequestedAt,
        };
    }

    public static ConversionRequestEntityV1[] ToRequestEntities(this IEnumerable<ConversionRequest> requests) =>
        requests.ToArrayBy(ToRequestEntity);

    private static ConversionRequest ToDomainRequest(this ConversionRequestEntityV1 entity)
    {
        return new ConversionRequest
        {
            ItemId = entity.ItemId,
            RegistrationId = entity.RegistrationId,
            ConversionDateFrom = entity.RequestFrom,
            ConversionDateTo = entity.RequestTo,
            RequestedAt = entity.RequestedAt
        };
    }

    public static ConversionRequest[] ToDomainRequests(this IEnumerable<ConversionRequestEntityV1> entities) =>
        entities.ToArrayBy(ToDomainRequest);
}