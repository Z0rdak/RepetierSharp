using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RepetierSharp.Config
{
    public class Shape
    {
        [JsonPropertyName("basicShape")]
        public BasicShape BasicShape { get; }

        [JsonPropertyName("gridColor")]
        public string GridColor { get; }

        [JsonPropertyName("gridSpacing")]
        public int GridSpacing { get; }

        [JsonPropertyName("marker")]
        public List<object> Marker { get; }
    }

}
