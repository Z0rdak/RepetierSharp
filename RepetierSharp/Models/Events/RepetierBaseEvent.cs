using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Events
{
    public class RepetierBaseEvent
    {
        [JsonPropertyName("event")] public string Event { get; set; }

        [JsonPropertyName("printer")] public string Printer { get; set; }

        [JsonPropertyName("data")] public IRepetierEvent RepetierEvent { get; set; }
    }

    public class RepetierBaseEventInfo
    {
        [JsonPropertyName("event")] public string Event { get; set; }

        [JsonPropertyName("printer")] public string Printer { get; set; }
    }
}
