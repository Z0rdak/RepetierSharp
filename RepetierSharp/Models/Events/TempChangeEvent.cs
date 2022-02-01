using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Events
{
    public class TempChangeEvent : IRepetierEvent
    {
        [JsonPropertyName("O")]
        public double Output { get; set; }

        [JsonPropertyName("S")]
        public double Set { get; set; }

        [JsonPropertyName("T")]
        public double Measured { get; set; }

        [JsonPropertyName("id")]
        public int ExtruderNo { get; set; }

        [JsonPropertyName("t")]
        public long Timestamp { get; set; }

        public TempChangeEvent() { }
    }

}