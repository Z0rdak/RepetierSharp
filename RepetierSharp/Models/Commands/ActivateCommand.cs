using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Commands
{
    public class ActivateCommand : ICommandData
    {
        public ActivateCommand(string printer)
        {
            PrinterSlug = printer;
        }

        [JsonPropertyName("printer")] public string PrinterSlug { get; }

        [JsonIgnore] public string Action => CommandConstants.ACTIVATE;
    }
}
