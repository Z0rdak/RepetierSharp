using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Communication
{
    public class IRepetierEvent
    {
        [JsonPropertyName("event")] public string Event { get; set; }

        [JsonPropertyName("printer")] public string Printer { get; set; }

        [JsonPropertyName("data")] public IEventData EventData { get; set; }
    }

    public class RepetierBaseEventInfo
    {
        [JsonPropertyName("event")] public string Event { get; set; }

        [JsonPropertyName("printer")] public string Printer { get; set; }
    }
}
