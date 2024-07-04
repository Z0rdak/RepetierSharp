using System.Text.Json.Serialization;

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
