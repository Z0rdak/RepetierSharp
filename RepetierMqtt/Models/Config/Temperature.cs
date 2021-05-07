using System.Text.Json.Serialization;

namespace RepetierMqtt.Config
{
    public class Temperature
    {
        [JsonPropertyName("name")]
        public string Name { get; }

        [JsonPropertyName("temp")]
        public int Temp { get; }
    }

}
