using System.Text.Json.Serialization;
using RepetierSharp.Models.Communication;

namespace RepetierSharp.Models.Responses
{
    public class StatusResponse : IResponseData
    {
        [JsonPropertyName("success")] public bool Success { get; set; }
    }
}
