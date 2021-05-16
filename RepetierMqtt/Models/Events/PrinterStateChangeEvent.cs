using RepetierMqtt.Models;

namespace RepetierMqtt.Models.Events
{
    public class PrinterStateChangeEvent : IRepetierEvent
    {
        public PrinterState PrinterState { get; }

        public PrinterStateChangeEvent() { }
    }

}