using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Requests
{
    public class SendMovesRequest : IRepetierRequest
    {
        [JsonIgnore]
        public string CommandIdentifier => CommandConstants.SEND_MOVES;

        public SendMovesRequest() { }

        public static SendMovesRequest Instance => new SendMovesRequest();
    }
}
