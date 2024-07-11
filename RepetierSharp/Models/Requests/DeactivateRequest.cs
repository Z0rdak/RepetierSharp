using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Requests
{
    public class DeactivateRequest : IRepetierRequest
    {
        public DeactivateRequest(string printer)
        {
            PrinterSlug = printer;
        }

        [JsonPropertyName("printer")] public string PrinterSlug { get; }

        [JsonIgnore] public string CommandIdentifier => CommandConstants.DEACTIVATE;
    }
}
