using System.Text.Json.Serialization;

namespace RepetierSharp.Models
{
    public class TempHistorySet
    {
        [JsonPropertyName("O")]
        public double Out { get; set; }

        [JsonPropertyName("S")]
        public double Set { get; set; }

        [JsonPropertyName("T")]
        public double Read { get; set; }

        [JsonPropertyName("t")]
        public long TimeStamp { get; set; }

        public TempHistorySet() { }
    }
}