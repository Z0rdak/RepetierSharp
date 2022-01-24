using RepetierSharp.Models.Commands;
using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Commands
{
    public class SendCommand : ICommandData
    {
        [JsonPropertyName("cmd")]
        public string GCode { get; }

        public string CommandIdentifier => CommandConstants.SEND;

        public SendCommand(string gcode)
        {
            GCode = gcode;
        }
    }
}