using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Events
{
    public class GcodeInfoUpdated : IRepetierEvent
    {
        [JsonPropertyName("modelId")]
        public int ModelId { get; set; }
        [JsonPropertyName("modelPath")]
        public string ModelPath { get; set; }
        [JsonPropertyName("slug")]
        public string Slug { get; set; }
    }
}
