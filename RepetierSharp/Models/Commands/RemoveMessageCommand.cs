using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Commands
{
    public class RemoveMessageCommand : ICommandData
    {
        public RemoveMessageCommand(int messageId, string a = "")
        {
            Id = messageId;
            A = a;
        }

        [JsonPropertyName("id")] public int Id { get; }

        [JsonPropertyName("a")] public string A { get; } // empty or unpause ?

        [JsonIgnore] public string Action => CommandConstants.REMOVE_MESSAGE;
    }
}
