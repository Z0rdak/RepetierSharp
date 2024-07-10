using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Requests
{
    public class ActivateRequest : IRepetierRequest
    {
        public ActivateRequest(string printer)
        {
            PrinterSlug = printer;
        }

        [JsonPropertyName("printer")] public string PrinterSlug { get; }

        [JsonIgnore] public string CommandIdentifier => CommandConstants.ACTIVATE;
    }
}
