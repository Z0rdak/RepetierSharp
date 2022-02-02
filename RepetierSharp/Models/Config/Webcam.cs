using System.Text.Json.Serialization;

namespace RepetierSharp.Config
{
    public class Webcam
    {
        [JsonPropertyName("dynamicUrl")]
        public string DynamicUrl { get; set; }

        [JsonPropertyName("forceSnapshotPosition")]
        public bool ForceSnapshotPosition { get; set; }

        [JsonPropertyName("method")]
        public int Method { get; set; }

        [JsonPropertyName("orientation")]
        public int Orientation { get; set; }

        [JsonPropertyName("pos")]
        public int Pos { get; set; }

        [JsonPropertyName("reloadInterval")]
        public double ReloadInterval { get; set; }

        [JsonPropertyName("snapshotDelay")]
        public int SnapshotDelay { get; set; }

        [JsonPropertyName("snapshotStabilizeTime")]
        public int SnapshotStabilizeTime { get; set; }

        [JsonPropertyName("snapshotX")]
        public double SnapshotX { get; set; }

        [JsonPropertyName("snapshotY")]
        public double SnapshotY { get; set; }

        [JsonPropertyName("staticUrl")]
        public string StaticUrl { get; set; }

        [JsonPropertyName("timelapseBitrate")]
        public int TimelapseBitrate { get; set; }

        [JsonPropertyName("timelapseFramerate")]
        public int TimelapseFramerate { get; set; }

        [JsonPropertyName("timelapseHeight")]
        public double TimelapseHeight { get; set; }

        [JsonPropertyName("timelapseInterval")]
        public double TimelapseInterval { get; set; }

        [JsonPropertyName("timelapseLayer")]
        public int TimelapseLayer { get; set; }

        [JsonPropertyName("timelapseMethod")]
        public int TimelapseMethod { get; set; }

        [JsonPropertyName("timelapseSelected")]
        public int TimelapseSelected { get; set; }
    }

}
