using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Events
{
    [EventId("settingChanged")]
    public class SettingChanged : IRepetierEvent
    {
        // TODO: list of settings
        [JsonPropertyName("key")] public string Key { get; set; }

        [JsonPropertyName("value")] public string Value { get; set; }
    }
}
