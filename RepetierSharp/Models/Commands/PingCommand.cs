using System.Text.Json.Serialization;
using RepetierSharp.Internal;
using RepetierSharp.Models.Communication;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Commands
{
    [CommandId(CommandConstants.PING)]
    public class PingCommand : ICommandData
    {
        private PingCommand() { }

        public static PingCommand Instance => new();

        [JsonIgnore] public string Action => CommandConstants.PING;
    }
}
