using TestTask.RequestEventsGenerator.Contracts;

namespace TestTask.RequestEventsGenerator;

internal sealed class ConversionRequestEventGenerator
{
    private const int AdditionalTime = 60 * 60 * 5;
    private const int MaxPrevTime = 60 * 60 * 3;

    public IEnumerable<ConversionRequestEvent> GenerateEvents(int eventsCount)
    {
        var random = new Random();

        var registrationIds = Enumerable
            .Range(start: 0, count: eventsCount)
            .Select(_ => (long) random.Next(minValue: 1, maxValue: 5_000))
            .ToArray();

        var itemIds = Enumerable
            .Range(start: 0, count: eventsCount)
            .Select(_ => (long) random.Next(minValue: 1, maxValue: 10_000))
            .ToArray();

        return Enumerable
            .Range(start: 0, count: eventsCount)
            .Select(counter => new ConversionRequestEvent
            {
                RegistrationId = random.GetItems(registrationIds, 1).First(),
                ItemId = random.GetItems(itemIds, 1).First(),
                ConversionDateFrom =
                    DateTime.UtcNow - TimeSpan.FromSeconds(2 * (eventsCount - counter + AdditionalTime)),
                ConversionDateTo = DateTime.UtcNow - TimeSpan.FromSeconds(eventsCount - counter + AdditionalTime),
                //RequestedAt = DateTime.UtcNow - TimeSpan.FromSeconds(Math.Max(eventsCount - counter, MaxPrevTime))
                RequestedAt = DateTime.UtcNow - TimeSpan.FromDays(1)
            });
    }
}