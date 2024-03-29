﻿using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Messages
{
    /* Message sent by the server as a response 
     * to a command or containing a event message */
    public class RepetierBaseMessage
    {
        [JsonPropertyName("callback_id")]
        public int CallBackId { get; set; }

        [JsonPropertyName("session")]
        public string SessionId { get; set; }

        //[JsonPropertyName("data")]
        [JsonIgnore]
        public byte[] Data { get; set; } // IRepetierMessage

        [JsonPropertyName("eventList")]
        public bool? HasEvents { get; set; }

        public RepetierBaseMessage() { }

    }
}
