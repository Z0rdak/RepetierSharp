using System.Collections.Generic;
using System.Text.Json.Serialization;
using RepetierSharp.Models.Events;

namespace RepetierSharp.Models.Messages
{
    public class UserListResponse : IRepetierResponse
    {
        [JsonPropertyName("data")]
        public UserListContainer UserListContainer { get; set; }

        public UserListResponse() { }
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
