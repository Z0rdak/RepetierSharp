using System.Text.Json.Serialization;

namespace RepetierSharp.Models
{
    public class PrinterInfo
    {
        [JsonPropertyName("active")]
        public bool IsActive { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("online")]
        public int IsOnline { get; set; }

        [JsonPropertyName("slug")]
        public string Slug { get; set; }

        public PrinterInfo() { }
    }
}