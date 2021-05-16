using RepetierMqtt.Models;
using RepetierMqtt.Models.Commands;
using System.Text.Json.Serialization;

namespace RepetierMqtt.Models.Commands
{
    public class CreateUserCommand : ICommandData
    {
        [JsonPropertyName("login")]
        public string User { get; }

        [JsonPropertyName("password")]
        public string Password { get; }

        [JsonPropertyName("permission")]
        public int Permission { get; }

        public string CommandIdentifier => CommandConstants.CREATE_USER;

        public CreateUserCommand(string user, string password, int permission)
        {
            this.User = user;
            this.Password = password;
            this.Permission = permission;
        }
    }
}