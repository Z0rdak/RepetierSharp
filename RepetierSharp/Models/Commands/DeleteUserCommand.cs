using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Commands
{
    public class DeleteUserCommand : IRepetierCommand
    {
        public DeleteUserCommand(string user)
        {
            User = user;
        }

        [JsonPropertyName("login")] public string User { get; }

        [JsonIgnore] public string CommandIdentifier => CommandConstants.DELETE_USER;
    }
}
