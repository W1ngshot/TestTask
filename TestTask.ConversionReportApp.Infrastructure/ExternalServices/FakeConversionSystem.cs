using TestTask.ConversionReportApp.Domain.InfrastructureInterfaces;
using TestTask.ConversionReportApp.Domain.Models;

namespace TestTask.ConversionReportApp.Infrastructure.ExternalServices;

public class FakeConversionSystem : IConversionSystem
{
    public Task<IEnumerable<CalculateConversionResponse>> CalculateConversionsAsync(
        IEnumerable<CalculateConversionRequest> requests,
        CancellationToken token)
    {
        return Task.FromResult(requests.Select(request => new CalculateConversionResponse
        {
            ItemId = request.ItemId,
            RegistrationId = request.RegistrationId,
            ConversionRatio = Random.Shared.NextDouble(),
            PaymentsCount = Random.Shared.NextInt64(1, 100_000),
            ConversionDateFrom = request.ConversionDateFrom,
            ConversionDateTo = request.ConversionDateTo
        }));
    }
}