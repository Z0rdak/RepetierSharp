using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RepetierSharp.Config
{
    public class Extruder
    {
        [JsonPropertyName("acceleration")]
        public double Acceleration { get; set; }

        [JsonPropertyName("cooldownPerSecond")]
        public double CooldownPerSecond { get; set; }

        [JsonPropertyName("eJerk")]
        public int EJerk { get; set; }

        [JsonPropertyName("extrudeSpeed")]
        public int ExtrudeSpeed { get; set; }

        [JsonPropertyName("filamentDiameter")]
        public double FilamentDiameter { get; set; }

        [JsonPropertyName("heatupPerSecond")]
        public double HeatupPerSecond { get; set; }

        [JsonPropertyName("lastTemp")]
        public int LastTemp { get; set; }

        [JsonPropertyName("maxSpeed")]
        public int MaxSpeed { get; set; }

        [JsonPropertyName("maxTemp")]
        public int MaxTemp { get; set; }

        [JsonPropertyName("offsetX")]
        public int OffsetX { get; set; }

        [JsonPropertyName("offsetY")]
        public int OffsetY { get; set; }

        [JsonPropertyName("retractSpeed")]
        public int RetractSpeed { get; set; }

        [JsonPropertyName("temperatures")]
        public List<Temperature> Temperatures { get; set; }
    }

}
