using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Commands
{
    public class ExtendPingCommand : ICommandData
    {
        [JsonIgnore]
        public string CommandIdentifier => CommandConstants.PING;

        public uint Timeout { get; set; }

        public ExtendPingCommand(uint timeout)
        {
            Timeout = timeout;
        }

    }
}
