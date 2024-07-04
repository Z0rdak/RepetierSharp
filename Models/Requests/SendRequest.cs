using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Requests
{
    public class SendRequest : IRepetierRequest
    {
        [JsonPropertyName("cmd")]
        public string GCode { get; }
        [JsonIgnore]
        public string CommandIdentifier => CommandConstants.SEND;

        public SendRequest(string gcode)
        {
            GCode = gcode;
        }
    }
}
