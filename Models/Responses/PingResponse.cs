using System.Text.Json.Serialization;
using RepetierSharp.Models.Events;

namespace RepetierSharp.Models.Messages
{
    public class PingResponse : IRepetierResponse
    {
        [JsonPropertyName("source")] // "source": "gui"
        public string Source { get; set; }

        public PingResponse() { }
    }
}
