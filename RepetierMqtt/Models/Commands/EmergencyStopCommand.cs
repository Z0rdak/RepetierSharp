using System;
using System.Collections.Generic;
using System.Text;

namespace RepetierMqtt.Models.Commands
{
    public class EmergencyStopCommand : ICommandData
    {
        public string CommandIdentifier => CommandConstants.EMERGENCY_STOP;

        private EmergencyStopCommand() { }

        public static EmergencyStopCommand Instance => new EmergencyStopCommand();
    }
}
