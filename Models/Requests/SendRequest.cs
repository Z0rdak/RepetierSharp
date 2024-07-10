using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Requests
{
    public class SendRequest : IRepetierRequest
    {
        public SendRequest(string gcode)
        {
            GCode = gcode;
        }

        [JsonPropertyName("cmd")] public string GCode { get; }

        [JsonIgnore] public string CommandIdentifier => CommandConstants.SEND;
    }
}
