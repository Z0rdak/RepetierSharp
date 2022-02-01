using RepetierSharp.Models;
using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Events
{
    [EventId("userCredentials")]
    public class UserCredentials : IRepetierEvent
    {
        [JsonPropertyName("login")]
        public string LoginName { get; set; }

        [JsonPropertyName("permissions")]
        public int PermissionLevel { get; set; }

        [JsonPropertyName("settings")]
        public UserSettings Settings { get; set; }

        public UserCredentials() { }
    }

}