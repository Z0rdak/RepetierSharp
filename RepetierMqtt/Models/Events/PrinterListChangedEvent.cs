using RepetierMqtt.Messages;

namespace RepetierMqtt.Event
{
    public class PrinterListChangedEvent : ListPrinterMessage
    {
        PrinterListChangedEvent() { }
        // Payload: List of printers like in listPrinters response.
    }

}