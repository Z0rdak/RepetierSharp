using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RepetierSharp.Config
{
    // TODO: base class for extruder, bed and chamber
    public class Extruder
    {
        [JsonPropertyName("acceleration")]
        public double Acceleration { get; set; }

        [JsonPropertyName("alias")]
        public string Alias { get; set; }

        [JsonPropertyName("changeFastDistance")]
        public double ChangeFastDistance { get; set; }

        [JsonPropertyName("changeSlowDistance")]
        public double ChangeSlowDistance { get; set; }

        [JsonPropertyName("cooldownPerSecond")]
        public double CooldownPerSecond { get; set; }

        [JsonPropertyName("eJerk")]
        public double EJerk { get; set; }

        [JsonPropertyName("extrudeSpeed")]
        public double ExtrudeSpeed { get; set; }

        [JsonPropertyName("filamentDiameter")]
        public double FilamentDiameter { get; set; }

        [JsonPropertyName("heatupPerSecond")]
        public double HeatupPerSecond { get; set; }

        [JsonPropertyName("lastTemp")]
        public int LastTemp { get; set; }

        [JsonPropertyName("maxSpeed")]
        public double MaxSpeed { get; set; }

        [JsonPropertyName("maxTemp")]
        public int MaxTemp { get; set; }

        [JsonPropertyName("num")]
        public int Num { get; set; }

        [JsonPropertyName("offset")]
        public double Offset { get; set; }

        [JsonPropertyName("offsetX")]
        public double OffsetX { get; set; }

        [JsonPropertyName("offsetY")]
        public double OffsetY { get; set; }

        [JsonPropertyName("retractSpeed")]
        public double RetractSpeed { get; set; }

        [JsonPropertyName("supportTemperature")]
        public bool SupportTemperature { get; set; }

        [JsonPropertyName("tempMaster")]
        public int TempMaster { get; set; }

        [JsonPropertyName("temperatures")]
        public List<Temperature> Temperatures { get; set; }

        [JsonPropertyName("toolDiameter")]
        public double ToolDiameter { get; set; }

        [JsonPropertyName("toolType")]
        public int ToolType { get; set; }
    }

}
