using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Requests
{
    public class RemoveMessageRequest : IRepetierRequest
    {
        public RemoveMessageRequest(int messageId, string a = "")
        {
            Id = messageId;
            A = a;
        }

        [JsonPropertyName("id")] public int Id { get; }

        [JsonPropertyName("a")] public string A { get; } // empty or unpause ?

        [JsonIgnore] public string CommandIdentifier => CommandConstants.REMOVE_MESSAGE;
    }
}
