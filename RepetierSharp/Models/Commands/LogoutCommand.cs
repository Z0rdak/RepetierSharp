using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Commands
{
    public class LogoutCommand : IRepetierCommand
    {
        private LogoutCommand() { }

        public static LogoutCommand Instance => new();

        [JsonIgnore] public string CommandIdentifier => CommandConstants.LOGOUT;
    }
}
