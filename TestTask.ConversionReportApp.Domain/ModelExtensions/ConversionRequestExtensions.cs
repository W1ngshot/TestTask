using TestTask.ConversionReportApp.Domain.Common;
using TestTask.ConversionReportApp.Domain.Models;

namespace TestTask.ConversionReportApp.Domain.ModelExtensions;

public static class ConversionRequestExtensions
{
    private static CalculateConversionRequest ToCalculateRequest(this ConversionRequest request)
    {
        return new CalculateConversionRequest
        {
            ItemId = request.ItemId,
            RegistrationId = request.RegistrationId,
            ConversionDateFrom = request.ConversionDateFrom,
            ConversionDateTo = request.ConversionDateTo
        };
    }

    public static CalculateConversionRequest[] ToCalculateRequests(this IEnumerable<ConversionRequest> requests) =>
        requests.ToArrayBy(ToCalculateRequest);
}