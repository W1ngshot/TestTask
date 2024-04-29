using System.Text.Json;
using System.Text.Json.Serialization;
using TestTask.RequestEventsGenerator;
using TestTask.RequestEventsGenerator.Contracts;
using TestTask.RequestEventsGenerator.Kafka;

const string bootstrapServers = "kafka:9092";
const string topicName = "report_request_events";
const int eventsCount = 10000;
const int timeoutMs = 5 * 60 * 1000;

using var cts = new CancellationTokenSource(timeoutMs);
var publisher = new KafkaPublisher<string, ConversionRequestEvent>(
    bootstrapServers,
    topicName,
    keySerializer: null,
    new SystemTextJsonSerializer<ConversionRequestEvent>(
        new JsonSerializerOptions {Converters = {new JsonStringEnumConverter()}}));

var generator = new ConversionRequestEventGenerator();

var messages = generator
    .GenerateEvents(eventsCount)
    .Select(@event => ($"{@event.RegistrationId}_{@event.ItemId}", @event))
    .ToList();

await publisher.Publish(messages, cts.Token);
Console.WriteLine("Successfully wrote messages to queue");