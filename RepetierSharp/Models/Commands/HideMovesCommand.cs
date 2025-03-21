using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Commands
{
    public class HideMovesCommand : ICommandData
    {
        public static HideMovesCommand Instance => new();

        [JsonIgnore] public string Action => CommandConstants.HIDE_MOVES;
    }
}
