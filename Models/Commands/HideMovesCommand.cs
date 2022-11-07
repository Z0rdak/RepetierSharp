using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Commands
{
    public class HideMovesCommand : ICommandData
    {
        [JsonIgnore]
        public string CommandIdentifier => CommandConstants.HIDE_MOVES;

        public HideMovesCommand() { }

        public static HideMovesCommand Instance => new HideMovesCommand();
    }
}
