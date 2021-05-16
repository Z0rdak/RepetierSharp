using RepetierMqtt.Models;
using RepetierMqtt.Models.Commands;
using System.Text.Json.Serialization;

namespace RepetierMqtt.Models.Commands
{
    public class UpdateUserCommand : ICommandData
    {
        [JsonPropertyName("login")]
        public string User { get; }

        [JsonPropertyName("password")]
        public string Password { get; }

        [JsonPropertyName("permission")]
        public int Permission { get; }

        public string CommandIdentifier => CommandConstants.UPDATE_USER;

        public UpdateUserCommand(string user, int permission, string password = "")
        {
            this.User = user;
            this.Password = password;
            this.Permission = permission;
        }
    }
}