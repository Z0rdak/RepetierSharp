using System.Text.Json.Serialization;
using RepetierSharp.Internal;
using RepetierSharp.Models.Communication;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Commands
{
    [CommandId(CommandConstants.EXTEND_PING)]
    public class ExtendPingCommand(uint timeout) : ICommandData
    {
        public uint Timeout { get; set; } = timeout;

        [JsonIgnore] public string Action => CommandConstants.EXTEND_PING;
    }
}
