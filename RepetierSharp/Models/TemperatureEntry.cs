using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RepetierSharp.Models
{
    public class TemperatureEntry
    {
        [JsonPropertyName("history")] public List<TempHistoryEntry> History { get; set; }

        [JsonPropertyName("error")] public int Error { get; set; }

        [JsonPropertyName("output")] public double Out { get; set; }

        [JsonPropertyName("tempRead")] public double Read { get; set; }

        [JsonPropertyName("tempSet")] public double Set { get; set; }
    }
}
