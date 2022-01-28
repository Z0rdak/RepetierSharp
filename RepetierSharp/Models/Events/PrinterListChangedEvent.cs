using RepetierSharp.Models.Messages;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Events
{
    public class PrinterListChangedEvent : IRepetierEvent
    {
        [JsonPropertyName("data")]
        public List<Printer> Printers { get; set; } = new List<Printer>();

        public PrinterListChangedEvent() { }
        // Payload: List of printers like in listPrinters response.
    }

}