using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Events
{
    public class TempChangeEvent : IRepetierEvent
    {
        [JsonPropertyName("O")]
        public double Output { get; }

        [JsonPropertyName("S")]
        public double Set { get; }

        [JsonPropertyName("T")]
        public double Measured { get; }

        [JsonPropertyName("id")]
        public int ExtruderNo { get; }

        [JsonPropertyName("t")]
        public long Timestamp { get; }

        public TempChangeEvent() { }
    }

}