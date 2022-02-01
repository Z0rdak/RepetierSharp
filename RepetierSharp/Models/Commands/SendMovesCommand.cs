using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Commands
{
    public class SendMovesCommand : ICommandData
    {
        [JsonIgnore]
        public string CommandIdentifier => CommandConstants.SEND_MOVES;

        public SendMovesCommand() { }

        public static SendMovesCommand Instance => new SendMovesCommand();
    }
}
