using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Commands
{
    public class MessagesCommand : ICommandData
    {
        [JsonIgnore]
        public string CommandIdentifier => CommandConstants.MESSAGES;

        private MessagesCommand() { }

        public static MessagesCommand Instance => new MessagesCommand();
    }
}
