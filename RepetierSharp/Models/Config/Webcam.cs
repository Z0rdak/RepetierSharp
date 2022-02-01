using System.Text.Json.Serialization;

namespace RepetierSharp.Config
{
    public class Webcam
    {
        [JsonPropertyName("dynamicUrl")]
        public string DynamicUrl { get; set; }

        [JsonPropertyName("method")]
        public int Method { get; set; }

        [JsonPropertyName("reloadInterval")]
        public int ReloadInterval { get; set; }

        [JsonPropertyName("staticUrl")]
        public string StaticUrl { get; set; }

        [JsonPropertyName("timelapseInterval")]
        public int TimelapseInterval { get; set; }

        [JsonPropertyName("timelapseMethod")]
        public int TimelapseMethod { get; set; }
    }

}
