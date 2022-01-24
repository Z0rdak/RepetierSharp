using RepetierSharp.Models.Commands;

namespace RepetierSharp.Models.Commands
{
    public class LogoutCommand : ICommandData
    {
        
        public string CommandIdentifier => CommandConstants.LOGOUT;

        private LogoutCommand() { }

        public static LogoutCommand Instance => new LogoutCommand();
    }
}