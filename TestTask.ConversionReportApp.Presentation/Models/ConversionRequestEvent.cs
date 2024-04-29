using System.Text.Json.Serialization;

namespace TestTask.ConversionReportApp.Presentation.Models;

public sealed class ConversionRequestEvent
{
    [JsonPropertyName("item_id")]
    public long ItemId { get; init; }
    
    [JsonPropertyName("registration_id")]
    public long RegistrationId { get; init; }
    
    [JsonPropertyName("conversion_date_from")]
    public DateTime ConversionDateFrom { get; init; }
    
    [JsonPropertyName("conversion_date_to")]
    public DateTime ConversionDateTo { get; init; }
    
    [JsonPropertyName("requested_at")]
    public DateTime RequestedAt { get; init; }
}
