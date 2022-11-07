using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Events
{
    [EventId("eepromData")]
    public class EepromData : IRepetierEvent
    {
        [JsonPropertyName("pos")]
        public string Pos { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonPropertyName("valueOrig")]
        public string ValueOrig { get; set; }

        public EepromData() { }
    }
}
