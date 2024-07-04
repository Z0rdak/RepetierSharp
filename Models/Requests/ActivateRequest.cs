using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Requests
{
    public class ActivateRequest : IRepetierRequest
    {
        [JsonPropertyName("printer")]
        public string PrinterSlug { get; }
        [JsonIgnore]
        public string CommandIdentifier => CommandConstants.ACTIVATE;

        public ActivateRequest(string printer)
        {
            PrinterSlug = printer;
        }
    }
}
