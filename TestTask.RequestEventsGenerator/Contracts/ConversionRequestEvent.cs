using System.Text.Json.Serialization;

namespace TestTask.RequestEventsGenerator.Contracts;

public sealed class ConversionRequestEvent
{
    [JsonPropertyName("item_id")]
    public long ItemId { get; set; }
    
    [JsonPropertyName("registration_id")]
    public long RegistrationId { get; set; }
    
    [JsonPropertyName("conversion_date_from")]
    public DateTime ConversionDateFrom { get; set; }
    
    [JsonPropertyName("conversion_date_to")]
    public DateTime ConversionDateTo { get; set; }
    
    [JsonPropertyName("requested_at")]
    public DateTime RequestedAt { get; set; }
}
