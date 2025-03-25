using System.Text.Json.Serialization;
using RepetierSharp.Internal;
using RepetierSharp.Models.Communication;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Commands
{
    [CommandId(CommandConstants.CONTINUE_JOB)]
    public class ContinueJobCommand : ICommandData
    {
        private ContinueJobCommand() { }

        public static ContinueJobCommand Instance => new();

        [JsonIgnore] public string Action => CommandConstants.CONTINUE_JOB;
    }
}
