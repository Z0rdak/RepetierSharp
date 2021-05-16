using RepetierMqtt.Models.Messages;

namespace RepetierMqtt.Models.Events
{
    public class PrinterListChangedEvent : ListPrinterMessage
    {
        PrinterListChangedEvent() { }
        // Payload: List of printers like in listPrinters response.
    }

}