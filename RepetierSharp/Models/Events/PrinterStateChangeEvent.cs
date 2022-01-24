using RepetierSharp.Models;

namespace RepetierSharp.Models.Events
{
    public class PrinterStateChangeEvent : IRepetierEvent
    {
        public PrinterState PrinterState { get; }

        public PrinterStateChangeEvent() { }
    }

}