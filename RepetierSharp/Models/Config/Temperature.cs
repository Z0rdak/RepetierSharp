using System.Text.Json.Serialization;

namespace RepetierSharp.Config
{
    public class Temperature
    {
        [JsonPropertyName("name")]
        public string Name { get; }

        [JsonPropertyName("temp")]
        public int Temp { get; }
    }

}
