using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Commands
{
    public class CreateUserCommand : ICommandData
    {
        [JsonPropertyName("login")]
        public string User { get; }

        [JsonPropertyName("password")]
        public string Password { get; }

        [JsonPropertyName("permission")]
        public int Permission { get; }
        [JsonIgnore]
        public string CommandIdentifier => CommandConstants.CREATE_USER;

        public CreateUserCommand(string user, string password, int permission)
        {
            this.User = user;
            this.Password = password;
            this.Permission = permission;
        }
    }
}
