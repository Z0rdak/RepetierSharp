using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Commands
{
    public class LogoutCommand : ICommandData
    {
        [JsonIgnore]
        public string CommandIdentifier => CommandConstants.LOGOUT;

        private LogoutCommand() { }

        public static LogoutCommand Instance => new LogoutCommand();
    }
}
