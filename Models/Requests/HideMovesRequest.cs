using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Requests
{
    public class HideMovesRequest : IRepetierRequest
    {
        public static HideMovesRequest Instance => new();

        [JsonIgnore] public string CommandIdentifier => CommandConstants.HIDE_MOVES;
    }
}
