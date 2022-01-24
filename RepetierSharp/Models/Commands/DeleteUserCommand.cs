using RepetierSharp.Models;
using RepetierSharp.Models.Commands;
using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Commands
{
    public class DeleteUserCommand : ICommandData
    {
        [JsonPropertyName("login")]
        public string User { get; }

        public string CommandIdentifier => CommandConstants.DELETE_USER;

        public DeleteUserCommand(string user)
        {
            this.User = user;
        }
    }
}