using RepetierMqtt.Models;
using RepetierMqtt.Models.Commands;
using System.Text.Json.Serialization;

namespace RepetierMqtt.Models.Commands
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