using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Requests
{
    public class EmergencyStopRequest : IRepetierRequest
    {
        [JsonIgnore]
        public string CommandIdentifier => CommandConstants.EMERGENCY_STOP;

        private EmergencyStopRequest() { }

        public static EmergencyStopRequest Instance => new EmergencyStopRequest();
    }
}
