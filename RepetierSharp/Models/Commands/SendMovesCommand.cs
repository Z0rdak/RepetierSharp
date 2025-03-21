using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Commands
{
    public class SendMovesCommand : ICommandData
    {
        public static SendMovesCommand Instance => new();

        [JsonIgnore] public string Action => CommandConstants.SEND_MOVES;
    }
}
