namespace RepetierMqtt.Models
{
    public class LogoutCommand : ICommandData
    {
        
        public string CommandIdentifier => "logout";

        private LogoutCommand() { }

        public static LogoutCommand Instance => new LogoutCommand();
    }
}