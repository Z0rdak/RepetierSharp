using System.Text.Json.Serialization;

namespace RepetierSharp.Config
{
    public class BasicShape
    {
        [JsonPropertyName("color")]
        public string Color { get; }

        [JsonPropertyName("radius")]
        public int Radius { get; }

        [JsonPropertyName("shape")]
        public string Shape { get; }

        [JsonPropertyName("x")]
        public int X { get; }

        [JsonPropertyName("xMax")]
        public int XMax { get; }

        [JsonPropertyName("xMin")]
        public int XMin { get; }

        [JsonPropertyName("y")]
        public int Y { get; }

        [JsonPropertyName("yMax")]
        public int YMax { get; }

        [JsonPropertyName("yMin")]
        public int YMin { get; }
    }

}
