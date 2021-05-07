using RepetierMqtt.Models;

namespace RepetierMqtt.Event
{
    public class PrinterStateChangeEvent : IRepetierEvent
    {
        public PrinterState PrinterState { get; }

        public PrinterStateChangeEvent() { }
    }

}