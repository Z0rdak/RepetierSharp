using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Events
{
    [EventId("printerListChanged")]
    public class PrinterListChanged : IRepetierEvent
    {
        [JsonPropertyName("data")] public List<Printer> Printers { get; set; } = new();
        // Payload: List of printers like in listPrinters response.
    }
}
