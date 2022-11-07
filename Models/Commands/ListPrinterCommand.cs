using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Commands
{
    public class ListPrinterCommand : ICommandData
    {
        [JsonIgnore]
        public string CommandIdentifier => CommandConstants.LIST_PRINTER;

        private ListPrinterCommand() { }

        public static ListPrinterCommand Instance => new ListPrinterCommand();
    }
}
