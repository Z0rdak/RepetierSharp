using RepetierMqtt.Messages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace RepetierMqtt.Models.Messages
{
    public class UserListMessage : IRepetierMessage
    {
        [JsonPropertyName("data")]
        public UserListContainer UserListContainer { get; }

        public UserListMessage() { }
    }

    public class UserListContainer
    {
        [JsonPropertyName("loginRequired")]
        public bool LoginRequired { get; set; }

        [JsonPropertyName("users")]
        public List<User> Users { get; set; }

        public UserListContainer() { }
    }
}
