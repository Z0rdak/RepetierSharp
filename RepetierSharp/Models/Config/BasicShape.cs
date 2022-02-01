using System.Text.Json.Serialization;

namespace RepetierSharp.Config
{
    public class BasicShape
    {
        [JsonPropertyName("color")]
        public string Color { get; set; }

        [JsonPropertyName("radius")]
        public int Radius { get; set; }

        [JsonPropertyName("shape")]
        public string Shape { get; set; }

        [JsonPropertyName("x")]
        public int X { get; set; }

        [JsonPropertyName("xMax")]
        public int XMax { get; set; }

        [JsonPropertyName("xMin")]
        public int XMin { get; set; }

        [JsonPropertyName("y")]
        public int Y { get; set; }

        [JsonPropertyName("yMax")]
        public int YMax { get; set; }

        [JsonPropertyName("yMin")]
        public int YMin { get; set; }
    }

}
