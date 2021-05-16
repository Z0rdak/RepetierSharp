using RepetierMqtt.Models.Commands;

namespace RepetierMqtt.Models.Commands
{
    public class LogoutCommand : ICommandData
    {
        
        public string CommandIdentifier => CommandConstants.LOGOUT;

        private LogoutCommand() { }

        public static LogoutCommand Instance => new LogoutCommand();
    }
}