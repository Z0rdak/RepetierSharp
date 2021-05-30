using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace RepetierMqtt.Models.Events
{
    public class EepromDataEvent : IRepetierEvent
    {
        [JsonPropertyName("pos")]
        public string Pos { get; set; }

        [JsonPropertyName("text")]
        public string Text { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; }

        [JsonPropertyName("value")]
        public string Value { get; set; }

        [JsonPropertyName("valueOrig")]
        public string ValueOrig { get; set; }

        public EepromDataEvent() { }
    }
}
