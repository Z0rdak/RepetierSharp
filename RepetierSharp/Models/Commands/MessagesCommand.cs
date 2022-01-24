using System;
using System.Collections.Generic;
using System.Text;

namespace RepetierSharp.Models.Commands
{
    public class MessagesCommand : ICommandData
    {
        public string CommandIdentifier => CommandConstants.MESSAGES;

        private MessagesCommand() { }

        public static MessagesCommand Instance => new MessagesCommand();
    }
}
