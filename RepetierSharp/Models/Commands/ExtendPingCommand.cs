using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Commands
{
    public class ExtendPingCommand : IRepetierCommand
    {
        public ExtendPingCommand(uint timeout)
        {
            Timeout = timeout;
        }

        public uint Timeout { get; set; }

        [JsonIgnore] public string CommandIdentifier => CommandConstants.PING;
    }
}
