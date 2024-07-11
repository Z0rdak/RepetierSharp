using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Commands
{
    public class UpdateUserCommand : IRepetierCommand
    {
        public UpdateUserCommand(string user, int permission, string password = "")
        {
            User = user;
            Password = password;
            Permission = permission;
        }

        [JsonPropertyName("login")] public string User { get; }

        [JsonPropertyName("password")] public string Password { get; }

        [JsonPropertyName("permission")] public int Permission { get; }

        [JsonIgnore] public string CommandIdentifier => CommandConstants.UPDATE_USER;
    }
}
