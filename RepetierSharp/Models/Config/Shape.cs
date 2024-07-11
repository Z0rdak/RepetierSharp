using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RepetierSharp.Config
{
    public class Shape
    {
        [JsonPropertyName("basicShape")] public BasicShape BasicShape { get; set; }

        [JsonPropertyName("gridColor")] public string GridColor { get; set; }

        [JsonPropertyName("gridSpacing")] public double GridSpacing { get; set; }

        // TODO: type unknown
        [JsonPropertyName("marker")] public List<object> Marker { get; set; }
    }

    public class BasicShape
    {
        [JsonPropertyName("angle")] public double Angle { get; set; }

        [JsonPropertyName("color")] public string Color { get; set; }

        [JsonPropertyName("radius")] public double Radius { get; set; }

        [JsonPropertyName("shape")] public string Shape { get; set; }

        [JsonPropertyName("x")] public double X { get; set; }

        [JsonPropertyName("xMax")] public double XMax { get; set; }

        [JsonPropertyName("xMin")] public double XMin { get; set; }

        [JsonPropertyName("y")] public double Y { get; set; }

        [JsonPropertyName("yMax")] public double YMax { get; set; }

        [JsonPropertyName("yMin")] public double YMin { get; set; }
    }
}
