using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Events
{
    public class RepetierBaseEvent
    {
        [JsonPropertyName("event")]
        public string Event { get; set; }

        [JsonPropertyName("printer")]
        public string Printer { get; set; }

        // [JsonPropertyName("data")]
        [JsonIgnore]
        public byte[] Data { get; set; } // IRepetierEvent

        public RepetierBaseEvent() { }
    }
}
