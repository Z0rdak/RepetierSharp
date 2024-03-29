﻿using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Commands
{
    public class DeleteUserCommand : ICommandData
    {
        [JsonPropertyName("login")]
        public string User { get; }
        [JsonIgnore]
        public string CommandIdentifier => CommandConstants.DELETE_USER;

        public DeleteUserCommand(string user)
        {
            this.User = user;
        }
    }
}
