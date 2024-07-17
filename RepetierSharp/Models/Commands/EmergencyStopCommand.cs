using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Commands
{
    public class EmergencyStopCommand : IRepetierCommand
    {
        private EmergencyStopCommand() { }

        public static EmergencyStopCommand Instance => new();

        [JsonIgnore] public string CommandIdentifier => CommandConstants.EMERGENCY_STOP;
    }
}
