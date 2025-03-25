using System.Text.Json.Serialization;
using RepetierSharp.Models.Communication;

namespace RepetierSharp.Models.Responses
{
    public class PingResponse : IResponseData
    {
        [JsonPropertyName("source")] public string Source { get; set; }
    }
}
