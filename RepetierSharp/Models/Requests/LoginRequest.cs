using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Requests
{
    public class LoginRequest : IRepetierRequest
    {
        public LoginRequest(string name, string password, bool longLived = false)
        {
            LoginName = name;
            Password = password;
            LongLivedSession = longLived;
        }

        [JsonPropertyName("login")] public string LoginName { get; private set; }

        [JsonPropertyName("password")]
        public string Password { get; private set; } //  Password is MD5(sessionId + MD5(login + password)) 

        [JsonPropertyName("rememberMe")] public bool? LongLivedSession { get; private set; }

        [JsonIgnore] public string CommandIdentifier => CommandConstants.LOGIN;
    }
}
