using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RepetierSharp.Config
{
    public class HeatedBed
    {
        [JsonPropertyName("cooldownPerSecond")]
        public int CooldownPerSecond { get; set; }

        [JsonPropertyName("heatupPerSecond")]
        public int HeatupPerSecond { get; set; }

        [JsonPropertyName("installed")]
        public bool Installed { get; set; }

        [JsonPropertyName("lastTemp")]
        public int LastTemp { get; set; }

        [JsonPropertyName("maxTemp")]
        public int MaxTemp { get; set; }

        [JsonPropertyName("temperatures")]
        public List<Temperature> Temperatures { get; set; }
    }

}
