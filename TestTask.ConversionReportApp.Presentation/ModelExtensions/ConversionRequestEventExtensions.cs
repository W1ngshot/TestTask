using TestTask.ConversionReportApp.Domain.Models;
using TestTask.ConversionReportApp.Presentation.Models;

namespace TestTask.ConversionReportApp.Presentation.ModelExtensions;

public static class ConversionRequestEventExtensions
{
    private static ConversionRequest ToDomainRequest(this ConversionRequestEvent requestEvent)
    {
        return new ConversionRequest
        {
            ItemId = requestEvent.ItemId,
            RegistrationId = requestEvent.RegistrationId,
            ConversionDateFrom = requestEvent.ConversionDateFrom,
            ConversionDateTo = requestEvent.ConversionDateTo,
            RequestedAt = requestEvent.RequestedAt
        };
    }

    public static IEnumerable<ConversionRequest> ToDomainRequests(
        this IEnumerable<ConversionRequestEvent> requestEvents) => requestEvents.Select(ToDomainRequest);
}