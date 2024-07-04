using System.Text.Json.Serialization;

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
