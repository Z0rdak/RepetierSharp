using RepetierSharp.Models;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Messages
{
    public class ListPrinterMessage : IRepetierMessage
    {
        [JsonPropertyName("data")]
        public Dictionary<string, PrinterState> Printers { get; set; } = new Dictionary<string, PrinterState>();

        public ListPrinterMessage() { }
    }
}