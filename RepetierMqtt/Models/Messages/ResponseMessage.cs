using RepetierMqtt.Messages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace RepetierMqtt.Models.Messages
{
    public class ResponseMessage : IRepetierMessage
    {

        [JsonPropertyName("lastid")]
        public int Lastid { get; }

        [JsonPropertyName("lines")]
        public List<Line> Lines { get; }

        [JsonPropertyName("state")]
        public PrinterState PrinterState { get; }

        public ResponseMessage() { }
    }
}
