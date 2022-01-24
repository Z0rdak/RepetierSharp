using System.Text.Json.Serialization;

namespace RepetierSharp.Config
{
    public class Webcam
    {
        [JsonPropertyName("dynamicUrl")]
        public string DynamicUrl { get; }

        [JsonPropertyName("method")]
        public int Method { get; }

        [JsonPropertyName("reloadInterval")]
        public int ReloadInterval { get; }

        [JsonPropertyName("staticUrl")]
        public string StaticUrl { get; }

        [JsonPropertyName("timelapseInterval")]
        public int TimelapseInterval { get; }

        [JsonPropertyName("timelapseMethod")]
        public int TimelapseMethod { get; }
    }

}
