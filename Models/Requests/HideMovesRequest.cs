using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Requests
{
    public class HideMovesRequest : IRepetierRequest
    {
        [JsonIgnore]
        public string CommandIdentifier => CommandConstants.HIDE_MOVES;

        public HideMovesRequest() { }

        public static HideMovesRequest Instance => new HideMovesRequest();
    }
}
