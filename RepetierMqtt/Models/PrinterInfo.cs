using System.Text.Json.Serialization;

namespace RepetierMqtt.Models
{
    public class PrinterInfo
    {
        [JsonPropertyName("active")]
        public bool IsActive { get; }

        [JsonPropertyName("name")]
        public string Name { get; }

        [JsonPropertyName("online")]
        public int IsOnline { get; }

        [JsonPropertyName("slug")]
        public string Slug { get; }
    }
}