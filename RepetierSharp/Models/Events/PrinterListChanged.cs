using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json.Serialization;
using RepetierSharp.Models.Communication;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Events
{
    [EventId(EventConstants.PRINTER_LIST_CHANGED)]
    public class PrinterListChanged : IEventData
    {
        [JsonPropertyName("data")] 
        public List<Printer> Printers { get; set; } = new();
    }

}
