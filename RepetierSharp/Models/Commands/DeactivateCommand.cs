using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Commands
{
    public class DeactivateCommand : ICommandData
    {
        public DeactivateCommand(string printer)
        {
            PrinterSlug = printer;
        }

        [JsonPropertyName("printer")] public string PrinterSlug { get; }

        [JsonIgnore] public string Action => CommandConstants.DEACTIVATE;
    }
}
