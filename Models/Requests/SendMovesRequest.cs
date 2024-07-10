using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Requests
{
    public class SendMovesRequest : IRepetierRequest
    {
        public static SendMovesRequest Instance => new();

        [JsonIgnore] public string CommandIdentifier => CommandConstants.SEND_MOVES;
    }
}
