using System.Text.Json.Serialization;
using RepetierSharp.Internal;
using RepetierSharp.Models.Communication;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Commands
{
    [CommandId(CommandConstants.REMOVE_MESSAGE)]
    public class RemoveMessageCommand(int messageId, string a = "") : ICommandData
    {
        [JsonPropertyName("id")] public int Id { get; } = messageId;

        [JsonPropertyName("a")] public string A { get; } = a; // empty or unpause ?

        [JsonIgnore] public string Action => CommandConstants.REMOVE_MESSAGE;
    }
}
