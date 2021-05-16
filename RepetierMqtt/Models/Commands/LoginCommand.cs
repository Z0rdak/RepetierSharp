using RepetierMqtt.Models.Commands;
using System.Text.Json.Serialization;

namespace RepetierMqtt.Models.Commands
{
    public class LoginCommand : ICommandData
    {
        [JsonPropertyName("login")]
        public string LoginName { get; private set; }

        [JsonPropertyName("password")]
        public string Password { get; private set; } //  Password is MD5(sessionId + MD5(login + password)) 

        public string CommandIdentifier => CommandConstants.LOGIN;

        public LoginCommand(string name, string password)
        {
            LoginName = name;
            Password = password;
        }
    }
}