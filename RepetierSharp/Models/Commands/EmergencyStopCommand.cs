using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Commands
{
    public class EmergencyStopCommand : ICommandData
    {
        private EmergencyStopCommand() { }

        public static EmergencyStopCommand Instance => new();

        [JsonIgnore] public string Action => CommandConstants.EMERGENCY_STOP;
    }
}
