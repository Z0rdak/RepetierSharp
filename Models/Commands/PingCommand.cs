using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Commands
{
    public class PingCommand : ICommandData
    {
        [JsonIgnore]
        public string CommandIdentifier => CommandConstants.PING;

        private PingCommand() { }

        public static PingCommand Instance => new PingCommand();
    }
}
