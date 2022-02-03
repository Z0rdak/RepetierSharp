using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Messages
{
    public class ResponseMessage : IRepetierMessage
    {

        [JsonPropertyName("lastid")]
        public int Lastid { get; set; }

        [JsonPropertyName("lines")]
        public List<Line> Lines { get; set; }

        [JsonPropertyName("state")]
        public PrinterState PrinterState { get; set; }

        public ResponseMessage() { }
    }
}
