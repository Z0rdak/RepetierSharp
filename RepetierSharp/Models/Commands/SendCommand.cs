using System.Text.Json.Serialization;
using RepetierSharp.Internal;
using RepetierSharp.Models.Communication;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Commands
{
    [CommandId(CommandConstants.SEND)]
    public class SendCommand(string gcode) : ICommandData
    {
        [JsonPropertyName("cmd")] public string GCode { get; } = gcode;

        [JsonIgnore] public string Action => CommandConstants.SEND;
    }
}
