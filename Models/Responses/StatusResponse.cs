using System.Text.Json.Serialization;
using RepetierSharp.Models.Events;

namespace RepetierSharp.Models.Messages
{
    public class StatusResponse : IRepetierResponse
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        public StatusResponse() { }
    }
}
