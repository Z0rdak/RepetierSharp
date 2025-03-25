using System.Text.Json.Serialization;
using RepetierSharp.Internal;
using RepetierSharp.Models.Communication;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Commands
{
    [CommandId(CommandConstants.LIST_PRINTER)]
    public class ListPrinterCommand : ICommandData
    {
        private ListPrinterCommand() { }

        public static ListPrinterCommand Instance => new();

        [JsonIgnore] public string Action => CommandConstants.LIST_PRINTER;
    }
}
