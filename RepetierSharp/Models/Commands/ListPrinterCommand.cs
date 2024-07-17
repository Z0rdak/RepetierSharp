using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Commands
{
    public class ListPrinterCommand : IRepetierCommand
    {
        private ListPrinterCommand() { }

        public static ListPrinterCommand Instance => new();

        [JsonIgnore] public string CommandIdentifier => CommandConstants.LIST_PRINTER;
    }
}
