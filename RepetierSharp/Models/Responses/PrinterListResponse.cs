using System.Collections.Generic;
using System.Text.Json.Serialization;
using RepetierSharp.Models.Communication;

namespace RepetierSharp.Models.Responses
{
    public class PrinterListResponse : IResponseData
    {
        [JsonPropertyName("data")] public List<Printer> Printer { get; set; } = new();
    }
}
