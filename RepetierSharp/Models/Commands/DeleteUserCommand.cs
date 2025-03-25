using System.Text.Json.Serialization;
using RepetierSharp.Internal;
using RepetierSharp.Models.Communication;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Commands
{
    [CommandId(CommandConstants.DELETE_USER)]
    public class DeleteUserCommand(string user) : ICommandData
    {
        [JsonPropertyName("login")] public string User { get; } = user;

        [JsonIgnore] public string Action => CommandConstants.DELETE_USER;
    }
}
