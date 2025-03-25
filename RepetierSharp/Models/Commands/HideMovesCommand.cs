using System.Text.Json.Serialization;
using RepetierSharp.Internal;
using RepetierSharp.Models.Communication;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Commands
{
    [CommandId(CommandConstants.HIDE_MOVES)]
    public class HideMovesCommand : ICommandData
    {
        public static HideMovesCommand Instance => new();

        [JsonIgnore] public string Action => CommandConstants.HIDE_MOVES;
    }
}
