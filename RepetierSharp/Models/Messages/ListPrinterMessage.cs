using RepetierSharp.Models;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Messages
{
    public class ListPrinterMessage : IRepetierMessage
    {
        [JsonPropertyName("data")]
        public List<Printer> Printers { get; set; } = new List<Printer>();

        public ListPrinterMessage() { }
    }
}