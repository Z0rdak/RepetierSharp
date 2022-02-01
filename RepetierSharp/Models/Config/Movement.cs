using System.Text.Json.Serialization;

namespace RepetierSharp.Config
{
    public class Movement
    {
        [JsonPropertyName("allEndstops")]
        public bool AllEndstops { get; set; }

        [JsonPropertyName("maxXYSpeed")]
        public int MaxXYSpeed { get; set; }

        [JsonPropertyName("maxZSpeed")]
        public int MaxZSpeed { get; set; }

        [JsonPropertyName("movebuffer")]
        public int Movebuffer { get; set; }

        [JsonPropertyName("timeMultiplier")]
        public int TimeMultiplier { get; set; }

        [JsonPropertyName("xEndstop")]
        public bool XEndstop { get; set; }

        [JsonPropertyName("xHome")]
        public int XHome { get; set; }

        [JsonPropertyName("xMax")]
        public int XMax { get; set; }

        [JsonPropertyName("xMin")]
        public int XMin { get; set; }

        [JsonPropertyName("xyJerk")]
        public int XyJerk { get; set; }

        [JsonPropertyName("xyPrintAcceleration")]
        public int XyPrintAcceleration { get; set; }

        [JsonPropertyName("xySpeed")]
        public int XySpeed { get; set; }

        [JsonPropertyName("xyTravelAcceleration")]
        public int XyTravelAcceleration { get; set; }

        [JsonPropertyName("yEndstop")]
        public bool YEndstop { get; set; }

        [JsonPropertyName("yHome")]
        public int YHome { get; set; }

        [JsonPropertyName("yMax")]
        public int YMax { get; set; }

        [JsonPropertyName("yMin")]
        public int YMin { get; set; }

        [JsonPropertyName("zEndstop")]
        public bool ZEndstop { get; set; }

        [JsonPropertyName("zHome")]
        public int ZHome { get; set; }

        [JsonPropertyName("zJerk")]
        public double ZJerk { get; set; }

        [JsonPropertyName("zMax")]
        public int ZMax { get; set; }

        [JsonPropertyName("zMin")]
        public int ZMin { get; set; }

        [JsonPropertyName("zPrintAcceleration")]
        public int ZPrintAcceleration { get; set; }

        [JsonPropertyName("zSpeed")]
        public int ZSpeed { get; set; }

        [JsonPropertyName("zTravelAcceleration")]
        public int ZTravelAcceleration { get; set; }
    }

}
