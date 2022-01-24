using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Events
{
    public class SettingChangedEvent : IRepetierEvent
    {
        [JsonPropertyName("key")]
        public string Key { get; set; }
        [JsonPropertyName("value")]
        public string Value { get; set; }

        public SettingChangedEvent() { }
    }
}
