using System.Text.Json.Serialization;
using RepetierSharp.Internal;
using RepetierSharp.Models.Communication;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Commands
{
    [CommandId(CommandConstants.UPDATE_USER)]
    public class UpdateUserCommand(string user, int permission, string password = "") : ICommandData
    {
        [JsonPropertyName("login")] public string User { get; } = user;

        [JsonPropertyName("password")] public string Password { get; } = password;

        [JsonPropertyName("permission")] public int Permission { get; } = permission;

        [JsonIgnore] public string Action => CommandConstants.UPDATE_USER;
    }
}
