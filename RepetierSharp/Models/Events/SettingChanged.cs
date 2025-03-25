using System.Collections.Generic;
using System.Text.Json.Serialization;
using RepetierSharp.Models.Communication;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Events
{
    [EventId(EventConstants.SETTING_CHANGED)]
    public class SettingChanged : IEventData
    {
        [JsonPropertyName("key")] public string Key { get; set; }

        [JsonPropertyName("value")] public string Value { get; set; }
    }
    
    public class SettingList
    {
        public List<SettingChanged> Settings { get; }
    }
}
