using System.Text.Json.Serialization;
using RepetierSharp.Internal;
using RepetierSharp.Models.Communication;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Commands
{
    [CommandId(CommandConstants.DEACTIVATE)]
    public class DeactivateCommand(string printer) : ICommandData
    {
        [JsonPropertyName("printer")] public string PrinterSlug { get; } = printer;

        [JsonIgnore] public string Action => CommandConstants.DEACTIVATE;
    }
}
