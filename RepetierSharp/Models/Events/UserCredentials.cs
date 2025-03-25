using System.Text.Json.Serialization;
using RepetierSharp.Models.Communication;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Events
{
    [EventId(EventConstants.USER_CREDENTIALS)]
    public class UserCredentials : IEventData
    {
        [JsonPropertyName("login")] public string LoginName { get; set; }

        [JsonPropertyName("permissions")] public int PermissionLevel { get; set; }

        [JsonPropertyName("settings")] public UserSettings Settings { get; set; }
    }
}
