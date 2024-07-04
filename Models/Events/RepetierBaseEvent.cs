﻿using System;
using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Events
{
    public class RepetierBaseEvent
    {
        [JsonPropertyName("event")]
        public string Event { get; set; }

        [JsonPropertyName("printer")]
        public string Printer { get; set; }

        [JsonPropertyName("data")]
        public IRepetierEvent Data { get; set; }

        public RepetierBaseEvent() { }
    }
    
    public class RepetierBaseEventInfo
    {
        [JsonPropertyName("event")]
        public string Event { get; set; }

        [JsonPropertyName("printer")]
        public string Printer { get; set; }
        
        public RepetierBaseEventInfo() { }
    }
}
