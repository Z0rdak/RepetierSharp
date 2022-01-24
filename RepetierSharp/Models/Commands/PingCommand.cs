using System;
using System.Collections.Generic;
using System.Text;

namespace RepetierSharp.Models.Commands
{
    public class PingCommand : ICommandData
    {
        public string CommandIdentifier => CommandConstants.PING;

        private PingCommand() { }

        public static PingCommand Instance => new PingCommand();
    }
}
