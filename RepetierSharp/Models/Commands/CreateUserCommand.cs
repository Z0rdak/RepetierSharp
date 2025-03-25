using System.Text.Json.Serialization;
using RepetierSharp.Internal;
using RepetierSharp.Models.Communication;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Commands
{
    [CommandId(CommandConstants.CREATE_USER)]
    public class CreateUserCommand(string user, string password, int permission) : ICommandData
    {
        [JsonPropertyName("login")] public string User { get; } = user;

        [JsonPropertyName("password")] public string Password { get; } = password;

        [JsonPropertyName("permission")] public int Permission { get; } = permission;

        [JsonIgnore] public string Action => CommandConstants.CREATE_USER;
    }
}
