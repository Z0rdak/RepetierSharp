using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Requests
{
    public class ListPrinterRequest : IRepetierRequest
    {
        [JsonIgnore]
        public string CommandIdentifier => CommandConstants.LIST_PRINTER;

        private ListPrinterRequest() { }

        public static ListPrinterRequest Instance => new ListPrinterRequest();
    }
}
