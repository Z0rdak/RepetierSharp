using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Requests
{
    public class EmergencyStopRequest : IRepetierRequest
    {
        private EmergencyStopRequest() { }

        public static EmergencyStopRequest Instance => new();

        [JsonIgnore] public string CommandIdentifier => CommandConstants.EMERGENCY_STOP;
    }
}
