using System.Text.Json.Serialization;

namespace RepetierSharp.Models
{
    public class Line
    {
        [JsonPropertyName("id")] public int Id { get; set; }

        [JsonPropertyName("text")] public string Text { get; set; }

        [JsonPropertyName("time")] public string Time { get; set; }

        [JsonPropertyName("type")] public int Type { get; set; }
    }
}
