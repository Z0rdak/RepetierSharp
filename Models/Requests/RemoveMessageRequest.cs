using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Requests
{
    public class RemoveMessageRequest : IRepetierRequest
    {
        [JsonPropertyName("id")]
        public int Id { get; }

        [JsonPropertyName("a")]
        public string A { get; } // empty or unpause ?
        [JsonIgnore]
        public string CommandIdentifier => CommandConstants.REMOVE_MESSAGE;

        public RemoveMessageRequest(int messageId, string a = "")
        {
            Id = messageId;
            A = a;
        }
    }
}
