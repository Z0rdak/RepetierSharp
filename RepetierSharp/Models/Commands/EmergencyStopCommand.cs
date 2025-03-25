using System.Text.Json.Serialization;
using RepetierSharp.Internal;
using RepetierSharp.Models.Communication;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Commands
{
    [CommandId(CommandConstants.EMERGENCY_STOP)]
    public class EmergencyStopCommand : ICommandData
    {
        private EmergencyStopCommand() { }

        public static EmergencyStopCommand Instance => new();

        [JsonIgnore] public string Action => CommandConstants.EMERGENCY_STOP;
    }
}
