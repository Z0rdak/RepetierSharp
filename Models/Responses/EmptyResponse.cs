using System.Text.Json.Serialization;
using RepetierSharp.Models.Events;

namespace RepetierSharp.Models.Messages
{
    public class EmptyResponse : IRepetierResponse
    {
        // no payload
    }
    
    public class OkResponse : IRepetierResponse
    {
        [JsonPropertyName("ok")]
        public bool Success { get; set; }
    }

    public class OkErrorResponse : OkResponse
    {
        [JsonPropertyName("error")]
        public string? Error { get; set; }
    }
}
