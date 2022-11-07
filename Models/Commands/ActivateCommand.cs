using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Commands
{
    public class ActivateCommand : ICommandData
    {
        [JsonPropertyName("printer")]
        public string PrinterSlug { get; }
        [JsonIgnore]
        public string CommandIdentifier => CommandConstants.ACTIVATE;

        public ActivateCommand(string printer)
        {
            PrinterSlug = printer;
        }
    }
}
