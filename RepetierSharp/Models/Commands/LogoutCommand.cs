using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Commands
{
    public class LogoutCommand : ICommandData
    {
        private LogoutCommand() { }

        public static LogoutCommand Instance => new();

        [JsonIgnore] public string Action => CommandConstants.LOGOUT;
    }
}
