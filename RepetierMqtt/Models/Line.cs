using System.Text.Json.Serialization;

namespace RepetierMqtt.Models.Messages
{

    public class Line
    {
        [JsonPropertyName("id")]
        public int Id { get; }

        [JsonPropertyName("text")]
        public string Text { get; }

        [JsonPropertyName("time")]
        public string Time { get; }

        [JsonPropertyName("type")]
        public int Type { get; }
    }

}
