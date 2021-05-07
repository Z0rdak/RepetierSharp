using System.Text.Json.Serialization;

namespace RepetierMqtt.Models
{
    public class Fan
    {
        [JsonPropertyName("on")]
        public bool On { get; }

        [JsonPropertyName("voltage")]
        public int Voltage { get; }

        public Fan() { }
    }
}