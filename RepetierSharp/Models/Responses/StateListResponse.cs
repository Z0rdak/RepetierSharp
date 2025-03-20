using System.Collections.Generic;
using System.Text.Json.Serialization;
using RepetierSharp.Models.Events;

namespace RepetierSharp.Models.Messages
{
    public class StateListResponse : IRepetierResponse
    {
        public Dictionary<string, PrinterState> PrinterStates { get; set; } = new();
    }
}
