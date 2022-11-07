using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Commands
{
    public class ContinueJobCommand : ICommandData
    {
        [JsonIgnore]
        public string CommandIdentifier => CommandConstants.CONTINUE_JOB;

        private ContinueJobCommand() { }

        public static ContinueJobCommand Instance => new ContinueJobCommand();
    }
}
