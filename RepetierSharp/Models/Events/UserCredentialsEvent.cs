using RepetierSharp.Models;
using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Events
{
    public class UserCredentialsEvent : IRepetierEvent
    {
        [JsonPropertyName("login")]
        public string LoginName { get; }

        [JsonPropertyName("permissions")]
        public int PermissionLevel { get; }

        [JsonPropertyName("settings")]
        public UserSettings Settings { get; }

        public UserCredentialsEvent() { }
    }

}