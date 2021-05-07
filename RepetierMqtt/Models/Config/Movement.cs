using System.Text.Json.Serialization;

namespace RepetierMqtt.Config
{
    public class Movement
    {
        [JsonPropertyName("allEndstops")]
        public bool AllEndstops { get; }

        [JsonPropertyName("maxXYSpeed")]
        public int MaxXYSpeed { get; }

        [JsonPropertyName("maxZSpeed")]
        public int MaxZSpeed { get; }

        [JsonPropertyName("movebuffer")]
        public int Movebuffer { get; }

        [JsonPropertyName("timeMultiplier")]
        public int TimeMultiplier { get; }

        [JsonPropertyName("xEndstop")]
        public bool XEndstop { get; }

        [JsonPropertyName("xHome")]
        public int XHome { get; }

        [JsonPropertyName("xMax")]
        public int XMax { get; }

        [JsonPropertyName("xMin")]
        public int XMin { get; }

        [JsonPropertyName("xyJerk")]
        public int XyJerk { get; }

        [JsonPropertyName("xyPrintAcceleration")]
        public int XyPrintAcceleration { get; }

        [JsonPropertyName("xySpeed")]
        public int XySpeed { get; }

        [JsonPropertyName("xyTravelAcceleration")]
        public int XyTravelAcceleration { get; }

        [JsonPropertyName("yEndstop")]
        public bool YEndstop { get; }

        [JsonPropertyName("yHome")]
        public int YHome { get; }

        [JsonPropertyName("yMax")]
        public int YMax { get; }

        [JsonPropertyName("yMin")]
        public int YMin { get; }

        [JsonPropertyName("zEndstop")]
        public bool ZEndstop { get; }

        [JsonPropertyName("zHome")]
        public int ZHome { get; }

        [JsonPropertyName("zJerk")]
        public double ZJerk { get; }

        [JsonPropertyName("zMax")]
        public int ZMax { get; }

        [JsonPropertyName("zMin")]
        public int ZMin { get; }

        [JsonPropertyName("zPrintAcceleration")]
        public int ZPrintAcceleration { get; }

        [JsonPropertyName("zSpeed")]
        public int ZSpeed { get; }

        [JsonPropertyName("zTravelAcceleration")]
        public int ZTravelAcceleration { get; }
    }

}
