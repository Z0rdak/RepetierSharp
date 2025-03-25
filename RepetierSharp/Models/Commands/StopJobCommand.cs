using System.Text.Json.Serialization;
using RepetierSharp.Internal;
using RepetierSharp.Models.Communication;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Commands
{
    [CommandId(CommandConstants.STOP_JOB)]
    public class StopJobCommand : ICommandData
    {
        private StopJobCommand() { }

        public static StopJobCommand Instance => new();

        [JsonIgnore] public string Action => CommandConstants.STOP_JOB;
    }
}
