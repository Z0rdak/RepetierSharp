using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Commands
{
    public class StopJobCommand : ICommandData
    {
        [JsonIgnore]
        public string CommandIdentifier => CommandConstants.STOP_JOB;

        private StopJobCommand() { }

        public static StopJobCommand Instance => new StopJobCommand();
    }
}
