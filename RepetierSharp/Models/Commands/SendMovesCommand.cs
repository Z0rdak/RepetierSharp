using System.Text.Json.Serialization;
using RepetierSharp.Internal;
using RepetierSharp.Models.Communication;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Commands
{
    [CommandId(CommandConstants.SEND_MOVES)]
    public class SendMovesCommand : ICommandData
    {
        public static SendMovesCommand Instance => new();

        [JsonIgnore] public string Action => CommandConstants.SEND_MOVES;
    }
}
