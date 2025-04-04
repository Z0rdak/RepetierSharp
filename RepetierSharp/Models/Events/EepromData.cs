﻿using System.Text.Json.Serialization;
using RepetierSharp.Models.Communication;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Events
{
    [EventId(EventConstants.EEPROM_DATA)]
    public class EepromData : IEventData
    {
        [JsonPropertyName("pos")] public string Pos { get; set; }
        [JsonPropertyName("text")] public string Text { get; set; }
        [JsonPropertyName("type")] public string Type { get; set; }
        [JsonPropertyName("value")] public string Value { get; set; }
        [JsonPropertyName("valueOrig")] public string ValueOrig { get; set; }
    }
}
