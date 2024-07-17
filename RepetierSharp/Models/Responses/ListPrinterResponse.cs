using System.Collections.Generic;
using RepetierSharp.Models.Events;

namespace RepetierSharp.Models.Messages
{
    public class ListPrinterResponse : IRepetierResponse
    {
        public List<Printer> Printers { get; set; } = new();
    }
}
