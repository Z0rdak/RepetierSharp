using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Commands
{
    public class EmergencyStopCommand : ICommandData
    {
        [JsonIgnore]
        public string CommandIdentifier => CommandConstants.EMERGENCY_STOP;

        private EmergencyStopCommand() { }

        public static EmergencyStopCommand Instance => new EmergencyStopCommand();
    }
}
