using System.Text.Json.Serialization;
using RepetierSharp.Internal;
using RepetierSharp.Models.Communication;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Commands
{
    [CommandId(CommandConstants.LOGOUT)]
    public class LogoutCommand : ICommandData
    {
        private LogoutCommand() { }

        public static LogoutCommand Instance => new();

        [JsonIgnore] public string Action => CommandConstants.LOGOUT;
    }
}
