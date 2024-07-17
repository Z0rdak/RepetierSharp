using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Commands
{
    public class ContinueJobCommand : IRepetierCommand
    {
        private ContinueJobCommand() { }

        public static ContinueJobCommand Instance => new();

        [JsonIgnore] public string CommandIdentifier => CommandConstants.CONTINUE_JOB;
    }
}
