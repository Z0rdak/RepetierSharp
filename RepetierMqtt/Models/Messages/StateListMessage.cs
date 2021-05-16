﻿using RepetierMqtt.Models;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RepetierMqtt.Models.Messages
{
    public class StateListMessage : IRepetierMessage
    {
        [JsonPropertyName("data")]
        public Dictionary<string, PrinterState> PrinterStates { get; }

        public StateListMessage() { }
    }
}