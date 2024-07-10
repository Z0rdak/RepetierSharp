using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Requests
{
    public class CreateUserRequest : IRepetierRequest
    {
        public CreateUserRequest(string user, string password, int permission)
        {
            User = user;
            Password = password;
            Permission = permission;
        }

        [JsonPropertyName("login")] public string User { get; }

        [JsonPropertyName("password")] public string Password { get; }

        [JsonPropertyName("permission")] public int Permission { get; }

        [JsonIgnore] public string CommandIdentifier => CommandConstants.CREATE_USER;
    }
}
