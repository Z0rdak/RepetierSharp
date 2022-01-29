using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Commands
{
    public class RemoveMessageCommand : ICommandData
    {
        [JsonPropertyName("id")]
        public int Id { get; }

        [JsonPropertyName("a")]
        public string A { get; } // empty or unpause ?
        [JsonIgnore]
        public string CommandIdentifier => CommandConstants.REMOVE_MESSAGE;

        public RemoveMessageCommand(int messageId, string a = "")
        {
            Id = messageId;
            A = a;
        }
    }
}
