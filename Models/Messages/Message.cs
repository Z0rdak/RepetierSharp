using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Messages
{
    public class Message : IRepetierMessage
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("msg")]
        public string Msg { get; set; }

        [JsonPropertyName("link")]
        public string FinishLink { get; set; }

        [JsonPropertyName("slug")]
        public string PrinterSlug { get; set; }

        // Date ISO 8601
        [JsonPropertyName("date")]
        public string DateString { get; set; }

        [JsonPropertyName("pause")]
        public bool IsPaused { get; set; }

        public Message() { }
    }
}
