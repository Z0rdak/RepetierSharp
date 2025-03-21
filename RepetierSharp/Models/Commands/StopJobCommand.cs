using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Commands
{
    public class StopJobCommand : ICommandData
    {
        private StopJobCommand() { }

        public static StopJobCommand Instance => new();

        [JsonIgnore] public string Action => CommandConstants.STOP_JOB;
    }
}
