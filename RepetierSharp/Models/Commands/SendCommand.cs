using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Commands
{
    public class SendCommand : ICommandData
    {
        public SendCommand(string gcode)
        {
            GCode = gcode;
        }

        [JsonPropertyName("cmd")] public string GCode { get; }

        [JsonIgnore] public string Action => CommandConstants.SEND;
    }
}
