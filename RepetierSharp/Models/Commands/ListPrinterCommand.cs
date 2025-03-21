using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Commands
{
    public class ListPrinterCommand : ICommandData
    {
        private ListPrinterCommand() { }

        public static ListPrinterCommand Instance => new();

        [JsonIgnore] public string Action => CommandConstants.LIST_PRINTER;
    }
}
