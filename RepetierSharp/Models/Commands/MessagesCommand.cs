using System.Text.Json.Serialization;
using RepetierSharp.Internal;
using RepetierSharp.Models.Communication;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Commands
{
    [CommandId(CommandConstants.MESSAGES)]
    public class MessagesCommand : ICommandData
    {
        private MessagesCommand() { }

        public static MessagesCommand Instance => new();

        [JsonIgnore] public string Action => CommandConstants.MESSAGES;
    }
}
