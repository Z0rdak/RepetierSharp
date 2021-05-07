using System;
using System.Collections.Generic;
using System.Text;

namespace RepetierMqtt.Models.Commands
{
    public class PingCommand : ICommandData
    {
        public string CommandIdentifier => "ping";

        private PingCommand() { }

        public static PingCommand Instance => new PingCommand();
    }
}
