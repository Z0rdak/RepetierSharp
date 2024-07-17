using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Commands
{
    public class StopJobCommand : IRepetierCommand
    {
        private StopJobCommand() { }

        public static StopJobCommand Instance => new();

        [JsonIgnore] public string CommandIdentifier => CommandConstants.STOP_JOB;
    }
}
