using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RepetierSharp.Models
{
    public class PrinterTemperature
    {
        [JsonPropertyName("history")] public List<TempHistorySet> History { get; set; }

        [JsonPropertyName("error")] public int Error { get; set; }

        [JsonPropertyName("output")] public double Out { get; set; }

        [JsonPropertyName("tempRead")] public double Read { get; set; }

        [JsonPropertyName("tempSet")] public double Set { get; set; }

        public override string ToString()
        {
            return $"{Read} -> {Set}";
        }
    }
}
