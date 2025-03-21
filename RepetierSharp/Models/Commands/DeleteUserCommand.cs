using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Commands
{
    public class DeleteUserCommand : ICommandData
    {
        public DeleteUserCommand(string user)
        {
            User = user;
        }

        [JsonPropertyName("login")] public string User { get; }

        [JsonIgnore] public string Action => CommandConstants.DELETE_USER;
    }
}
