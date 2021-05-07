using System;
using System.Collections.Generic;
using System.Text;

namespace RepetierMqtt.Models.Commands
{
    public class MessagesCommand : ICommandData
    {
        public string CommandIdentifier => "messages";

        private MessagesCommand() { }

        public static MessagesCommand Instance => new MessagesCommand();
    }
}
