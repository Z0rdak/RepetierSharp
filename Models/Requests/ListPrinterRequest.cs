using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Requests
{
    public class ListPrinterRequest : IRepetierRequest
    {
        private ListPrinterRequest() { }

        public static ListPrinterRequest Instance => new();

        [JsonIgnore] public string CommandIdentifier => CommandConstants.LIST_PRINTER;
    }
}
