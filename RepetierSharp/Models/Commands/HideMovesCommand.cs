using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Commands
{
    public class HideMovesCommand : IRepetierCommand
    {
        public static HideMovesCommand Instance => new();

        [JsonIgnore] public string CommandIdentifier => CommandConstants.HIDE_MOVES;
    }
}
