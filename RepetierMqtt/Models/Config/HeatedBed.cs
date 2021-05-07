using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RepetierMqtt.Config
{
    public class HeatedBed
    {
        [JsonPropertyName("cooldownPerSecond")]
        public int CooldownPerSecond { get; }

        [JsonPropertyName("heatupPerSecond")]
        public int HeatupPerSecond { get; }

        [JsonPropertyName("installed")]
        public bool Installed { get; }

        [JsonPropertyName("lastTemp")]
        public int LastTemp { get; }

        [JsonPropertyName("maxTemp")]
        public int MaxTemp { get; }

        [JsonPropertyName("temperatures")]
        public List<Temperature> Temperatures { get; }
    }

}
