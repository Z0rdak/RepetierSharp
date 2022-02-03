using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Messages
{
    public class PingMessage : IRepetierMessage
    {
        [JsonPropertyName("source")] // "source": "gui"
        public string Source { get; set; }

        public PingMessage() { }
    }
}