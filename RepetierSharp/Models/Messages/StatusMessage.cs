using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Messages
{
    public class StatusMessage : IRepetierMessage
    {
        [JsonPropertyName("success")]
        public bool Success { get; set; }

        public StatusMessage() { }
    }
}
