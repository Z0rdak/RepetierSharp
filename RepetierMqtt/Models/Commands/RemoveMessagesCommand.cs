using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace RepetierMqtt.Models.Commands
{
    public class RemoveMessagesCommand : ICommandData
    {
        [JsonPropertyName("id")]
        public int Id { get; }

        [JsonPropertyName("a")]
        public string A { get; } // empty or unpause ?

        public string CommandIdentifier => "removeMessage";

        public RemoveMessagesCommand(int messageId, string a = "")
        {
            Id = messageId;
            A = a;
        }
    }
}
