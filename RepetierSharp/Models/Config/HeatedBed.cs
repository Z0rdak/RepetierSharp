using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RepetierSharp.Config
{
    public class HeatedBed
    {
        [JsonPropertyName("alias")]
        public string Alias { get; set; }

        [JsonPropertyName("cooldownPerSecond")]
        public double CooldownPerSecond { get; set; }

        [JsonPropertyName("heatupPerSecond")]
        public double HeatupPerSecond { get; set; }

        [JsonPropertyName("lastTemp")]
        public int LastTemp { get; set; }

        [JsonPropertyName("maxTemp")]
        public int MaxTemp { get; set; }

        [JsonPropertyName("offset")]
        public int Offset { get; set; }

        [JsonPropertyName("temperatures")]
        public List<Temperature> Temperatures { get; set; }
    }

}
