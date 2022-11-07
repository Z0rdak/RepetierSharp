using System.Text.Json.Serialization;

namespace RepetierSharp.Models
{
    public class Fan
    {
        [JsonPropertyName("on")]
        public bool On { get; set; }

        [JsonPropertyName("voltage")]
        public int Voltage { get; set; }

        public Fan() { }
    }
}
