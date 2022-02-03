using System.Text.Json.Serialization;

namespace RepetierSharp.Config
{
    public class Movement
    {
        [JsonPropertyName("G10Distance")]
        public double G10Distance { get; set; }

        [JsonPropertyName("G10LongDistance")]
        public double G10LongDistance { get; set; }

        [JsonPropertyName("G10Speed")]
        public double G10Speed { get; set; }

        [JsonPropertyName("G10ZLift")]
        public double G10ZLift { get; set; }

        [JsonPropertyName("G11ExtraDistance")]
        public double G11ExtraDistance { get; set; }

        [JsonPropertyName("G11ExtraLongDistance")]
        public double G11ExtraLongDistance { get; set; }

        [JsonPropertyName("G11Speed")]
        public double G11Speed { get; set; }

        [JsonPropertyName("allEndstops")]
        public bool AllEndstops { get; set; }

        [JsonPropertyName("autoLevel")]
        public bool AutoLevel { get; set; }

        [JsonPropertyName("defaultAcceleration")]
        public double DefaultAcceleration { get; set; }

        [JsonPropertyName("defaultRetractAcceleration")]
        public double DefaultRetractAcceleration { get; set; }

        [JsonPropertyName("defaultTravelAcceleration")]
        public double DefaultTravelAcceleration { get; set; }

        [JsonPropertyName("invertX")]
        public bool InvertX { get; set; }

        [JsonPropertyName("invertY")]
        public bool InvertY { get; set; }

        [JsonPropertyName("invertZ")]
        public bool InvertZ { get; set; }

        [JsonPropertyName("maxXYSpeed")]
        public double MaxXYSpeed { get; set; }

        [JsonPropertyName("maxZSpeed")]
        public double MaxZSpeed { get; set; }

        [JsonPropertyName("movebuffer")]
        public int Movebuffer { get; set; }

        [JsonPropertyName("timeMultiplier")]
        public double TimeMultiplier { get; set; }

        [JsonPropertyName("xEndstop")]
        public bool XEndstop { get; set; }

        [JsonPropertyName("xHome")]
        public double XHome { get; set; }

        [JsonPropertyName("xMax")]
        public double XMax { get; set; }

        [JsonPropertyName("xMin")]
        public double XMin { get; set; }

        [JsonPropertyName("xyJerk")]
        public double XyJerk { get; set; }

        [JsonPropertyName("xyPrintAcceleration")]
        public double XyPrintAcceleration { get; set; }

        [JsonPropertyName("xySpeed")]
        public double XySpeed { get; set; }

        [JsonPropertyName("xyTravelAcceleration")]
        public double XyTravelAcceleration { get; set; }

        [JsonPropertyName("yEndstop")]
        public bool YEndstop { get; set; }

        [JsonPropertyName("yHome")]
        public double YHome { get; set; }

        [JsonPropertyName("yMax")]
        public double YMax { get; set; }

        [JsonPropertyName("yMin")]
        public double YMin { get; set; }

        [JsonPropertyName("zEndstop")]
        public bool ZEndstop { get; set; }

        [JsonPropertyName("zHome")]
        public double ZHome { get; set; }

        [JsonPropertyName("zJerk")]
        public double ZJerk { get; set; }

        [JsonPropertyName("zMax")]
        public double ZMax { get; set; }

        [JsonPropertyName("zMin")]
        public double ZMin { get; set; }

        [JsonPropertyName("zPrintAcceleration")]
        public double ZPrintAcceleration { get; set; }

        [JsonPropertyName("zSpeed")]
        public double ZSpeed { get; set; }

        [JsonPropertyName("zTravelAcceleration")]
        public double ZTravelAcceleration { get; set; }

        public Movement() { }
    }

}
