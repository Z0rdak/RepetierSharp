using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Responses
{
    public class OkErrorResponse : OkResponse
    {
        [JsonPropertyName("error")] public string? Error { get; set; }
    }
}
