using RepetierSharp.Models.Messages;

namespace RepetierSharp.Models.Events
{
    public class PrinterListChangedEvent : ListPrinterMessage, IRepetierEvent
    {
        PrinterListChangedEvent() { }
        // Payload: List of printers like in listPrinters response.
    }

}