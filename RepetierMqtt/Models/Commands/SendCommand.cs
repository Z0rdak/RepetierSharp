using RepetierMqtt.Models.Commands;
using System.Text.Json.Serialization;

namespace RepetierMqtt.Models
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