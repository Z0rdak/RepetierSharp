using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Commands
{
    public class DeactivateCommand : ICommandData
    {
        [JsonPropertyName("printer")]
        public string PrinterSlug { get; }
        [JsonIgnore]
        public string CommandIdentifier => CommandConstants.DEACTIVATE;

        public DeactivateCommand(string printer)
        {
            PrinterSlug = printer;
        }
    }
}
