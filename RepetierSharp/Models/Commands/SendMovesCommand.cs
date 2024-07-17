using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Commands
{
    public class SendMovesCommand : IRepetierCommand
    {
        public static SendMovesCommand Instance => new();

        [JsonIgnore] public string CommandIdentifier => CommandConstants.SEND_MOVES;
    }
}
