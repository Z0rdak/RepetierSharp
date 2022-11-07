using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Messages
{
    public class StateListMessage : IRepetierMessage
    {
        [JsonPropertyName("data")]
        public Dictionary<string, PrinterState> PrinterStates { get; set; } = new Dictionary<string, PrinterState>();

        public StateListMessage() { }
    }
}
