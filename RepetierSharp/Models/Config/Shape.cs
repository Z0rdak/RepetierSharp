using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RepetierSharp.Config
{
    public class Shape
    {
        [JsonPropertyName("basicShape")]
        public BasicShape BasicShape { get; set; }

        [JsonPropertyName("gridColor")]
        public string GridColor { get; set; }

        [JsonPropertyName("gridSpacing")]
        public int GridSpacing { get; set; }

        [JsonPropertyName("marker")]
        public List<object> Marker { get; set; }
    }

}
