namespace RepetierSharp.Models.Events
{
    [EventId("state")]
    public class PrinterStateChange : IRepetierEvent
    {
        public PrinterState PrinterState { get; set; }

        public PrinterStateChange() { }
    }

}