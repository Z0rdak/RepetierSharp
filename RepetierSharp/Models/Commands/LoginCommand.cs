using System.Text.Json.Serialization;
using RepetierSharp.Internal;
using RepetierSharp.Models.Communication;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Commands
{
    [CommandId(CommandConstants.LOGIN)]
    public class LoginCommand(string name, string password, bool longLived = false) : ICommandData
    {
        [JsonPropertyName("login")] public string LoginName { get; private set; } = name;

        [JsonPropertyName("password")]
        public string Password { get; private set; } = password; //  Password is MD5(sessionId + MD5(login + password)) 

        [JsonPropertyName("rememberMe")] public bool? LongLivedSession { get; private set; } = longLived;

        [JsonIgnore] public string Action => CommandConstants.LOGIN;
    }
}
