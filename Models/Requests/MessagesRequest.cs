using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Requests
{
    public class MessagesRequest : IRepetierRequest
    {
        [JsonIgnore]
        public string CommandIdentifier => CommandConstants.MESSAGES;

        private MessagesRequest() { }

        public static MessagesRequest Instance => new MessagesRequest();
    }
}
