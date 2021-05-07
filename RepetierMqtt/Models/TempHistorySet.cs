using System.Text.Json.Serialization;

namespace RepetierMqtt.Models
{
    public class TempHistorySet
    {
        [JsonPropertyName("O")]
        public int Out { get; set; }

        [JsonPropertyName("S")]
        public double Set { get; set; }

        [JsonPropertyName("T")]
        public double Read { get; set; }

        [JsonPropertyName("t")]
        public long TimeStamp { get; set; }
    }
}