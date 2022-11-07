using System.Collections.Generic;

namespace RepetierSharp.Models.Messages
{
    public class ListPrinterMessage : IRepetierMessage
    {
        public List<Printer> Printers { get; set; } = new List<Printer>();

        public ListPrinterMessage() { }
    }
}