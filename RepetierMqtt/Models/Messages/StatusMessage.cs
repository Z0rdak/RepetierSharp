using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace RepetierMqtt.Models.Messages
{
    public class StatusMessage
    {
        [JsonPropertyName("success")]
        public bool Success { get; }

        public StatusMessage() { }
    }
}
