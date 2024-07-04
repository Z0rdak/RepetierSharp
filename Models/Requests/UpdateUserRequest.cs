using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Requests
{
    public class UpdateUserRequest : IRepetierRequest
    {
        [JsonPropertyName("login")]
        public string User { get; }

        [JsonPropertyName("password")]
        public string Password { get; }

        [JsonPropertyName("permission")]
        public int Permission { get; }
        [JsonIgnore]
        public string CommandIdentifier => CommandConstants.UPDATE_USER;

        public UpdateUserRequest(string user, int permission, string password = "")
        {
            this.User = user;
            this.Password = password;
            this.Permission = permission;
        }
    }
}
