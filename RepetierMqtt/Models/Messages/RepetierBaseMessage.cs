using RepetierMqtt.Models.Messages;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using static RepetierMqtt.Models.RepetierBaseCommand;

namespace RepetierMqtt.Messages
{
    public class RepetierBaseMessage
    {
        [JsonPropertyName("callback_id")]
        public int CallBackId { get; }

        [JsonPropertyName("session")]
        public string SessionId { get; }

        [JsonPropertyName("data")]
        public byte[] Data { get; } // IRepetierMessage

        [JsonPropertyName("eventList")]
        public bool? HasEvents { get; }

        public RepetierBaseMessage() { }

    }
}