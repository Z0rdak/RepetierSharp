using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RepetierMqtt.Config
{
    public class Extruder
    {
        [JsonPropertyName("acceleration")]
        public int Acceleration { get; }

        [JsonPropertyName("cooldownPerSecond")]
        public double CooldownPerSecond { get; }

        [JsonPropertyName("eJerk")]
        public int EJerk { get; }

        [JsonPropertyName("extrudeSpeed")]
        public int ExtrudeSpeed { get; }

        [JsonPropertyName("filamentDiameter")]
        public double FilamentDiameter { get; }

        [JsonPropertyName("heatupPerSecond")]
        public double HeatupPerSecond { get; }

        [JsonPropertyName("lastTemp")]
        public int LastTemp { get; }

        [JsonPropertyName("maxSpeed")]
        public int MaxSpeed { get; }

        [JsonPropertyName("maxTemp")]
        public int MaxTemp { get; }

        [JsonPropertyName("offsetX")]
        public int OffsetX { get; }

        [JsonPropertyName("offsetY")]
        public int OffsetY { get; }

        [JsonPropertyName("retractSpeed")]
        public int RetractSpeed { get; }

        [JsonPropertyName("temperatures")]
        public List<Temperature> Temperatures { get; }
    }

}
