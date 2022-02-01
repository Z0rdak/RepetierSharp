using RepetierSharp.Models;
using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Events
{
    public class UserCredentialsEvent : IRepetierEvent
    {
        [JsonPropertyName("login")]
        public string LoginName { get; set; }

        [JsonPropertyName("permissions")]
        public int PermissionLevel { get; set; }

        [JsonPropertyName("settings")]
        public UserSettings Settings { get; set; }

        public UserCredentialsEvent() { }
    }

}