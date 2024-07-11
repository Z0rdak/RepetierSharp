using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Commands
{
    public class MessagesCommand : IRepetierCommand
    {
        private MessagesCommand() { }

        public static MessagesCommand Instance => new();

        [JsonIgnore] public string CommandIdentifier => CommandConstants.MESSAGES;
    }
}
