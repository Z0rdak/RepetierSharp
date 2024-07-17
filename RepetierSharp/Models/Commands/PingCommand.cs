using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Commands
{
    public class PingCommand : IRepetierCommand
    {
        private PingCommand() { }

        public static PingCommand Instance => new();

        [JsonIgnore] public string CommandIdentifier => CommandConstants.PING;
    }
}
