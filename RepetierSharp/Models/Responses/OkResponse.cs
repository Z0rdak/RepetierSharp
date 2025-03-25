using System.Text.Json.Serialization;
using RepetierSharp.Models.Communication;

namespace RepetierSharp.Models.Responses
{
    public class OkResponse : IResponseData
    {
        [JsonPropertyName("ok")] public bool Success { get; set; }
    }
}
