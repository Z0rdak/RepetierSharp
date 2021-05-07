using RepetierMqtt.Models;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RepetierMqtt.Messages
{
    public class LoginMessage : IRepetierMessage
    {
        [JsonPropertyName("ok")]
        public bool Authenticated { get; }

        [JsonPropertyName("permissions")]
        public int PermissionLevel { get; }

        [JsonPropertyName("login")]
        public string LoginName { get; }

        [JsonPropertyName("settings")]
        public UserSettings Settings { get; }

        public LoginMessage() { }
    }
}