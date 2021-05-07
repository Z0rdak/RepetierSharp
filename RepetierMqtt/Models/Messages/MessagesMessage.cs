using RepetierMqtt.Messages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace RepetierMqtt.Models.Messages
{
    public class MessagesMessage : IRepetierMessage
    {
        [JsonPropertyName("id")]
        public int Id { get; }

        [JsonPropertyName("msg")]
        public string Msg { get; }

        [JsonPropertyName("link")]
        public string FinishLink { get; }

        [JsonPropertyName("slug")]
        public string PrinterSlug { get; }

        // Date ISO 8601
        [JsonPropertyName("date")]
        public string DateString { get; }

        [JsonPropertyName("pause")]
        public bool IsPaused { get; }

        public MessagesMessage() { }
    }
}
