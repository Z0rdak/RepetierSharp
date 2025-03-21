using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Commands
{
    public class ExtendPingCommand : ICommandData
    {
        public ExtendPingCommand(uint timeout)
        {
            Timeout = timeout;
        }

        public uint Timeout { get; set; }

        [JsonIgnore] public string Action => CommandConstants.PING;
    }
}
