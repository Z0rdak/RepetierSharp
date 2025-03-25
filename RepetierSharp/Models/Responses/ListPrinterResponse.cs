using System.Collections.Generic;
using RepetierSharp.Models.Communication;

namespace RepetierSharp.Models.Responses
{
    public class ListPrinterResponse : IResponseData
    {
        public List<Printer> Printers { get; set; } = new();
    }
}
