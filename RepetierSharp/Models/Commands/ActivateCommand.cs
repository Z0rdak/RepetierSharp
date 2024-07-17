using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Commands
{
    public class ActivateCommand : IRepetierCommand
    {
        public ActivateCommand(string printer)
        {
            PrinterSlug = printer;
        }

        [JsonPropertyName("printer")] public string PrinterSlug { get; }

        [JsonIgnore] public string CommandIdentifier => CommandConstants.ACTIVATE;
    }
}
