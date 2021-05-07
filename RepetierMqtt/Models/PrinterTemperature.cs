using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RepetierMqtt.Models
{
    public class PrinterTemperature
    {
        [JsonPropertyName("history")]
        public List<TempHistorySet> History { get; }

        [JsonPropertyName("error")]
        public int Error { get; }

        [JsonPropertyName("output")]
        public double Out { get; }

        [JsonPropertyName("tempRead")]
        public double Read { get; }

        [JsonPropertyName("tempSet")]
        public double Set { get; }

        public override string ToString()
        {
            return $"{Read} -> {Set}";
        }
    }
}