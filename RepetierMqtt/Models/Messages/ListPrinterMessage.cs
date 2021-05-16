using RepetierMqtt.Models;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RepetierMqtt.Models.Messages
{
    public class ListPrinterMessage : IRepetierMessage
    {
        [JsonPropertyName("data")]
        public List<Printer> Printers { get; }

        public ListPrinterMessage() { }
    }
}