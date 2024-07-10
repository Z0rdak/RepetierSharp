using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Requests
{
    public class MessagesRequest : IRepetierRequest
    {
        private MessagesRequest() { }

        public static MessagesRequest Instance => new();

        [JsonIgnore] public string CommandIdentifier => CommandConstants.MESSAGES;
    }
}
