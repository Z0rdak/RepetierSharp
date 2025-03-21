using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Commands
{
    public class ContinueJobCommand : ICommandData
    {
        private ContinueJobCommand() { }

        public static ContinueJobCommand Instance => new();

        [JsonIgnore] public string Action => CommandConstants.CONTINUE_JOB;
    }
}
