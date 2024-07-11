using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Commands
{
    public class SendCommand : IRepetierCommand
    {
        public SendCommand(string gcode)
        {
            GCode = gcode;
        }

        [JsonPropertyName("cmd")] public string GCode { get; }

        [JsonIgnore] public string CommandIdentifier => CommandConstants.SEND;
    }
}
