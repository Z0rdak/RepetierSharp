namespace RepetierSharp.Models.Events
{
    [EventId("state")]
    public class PrinterStateChanged : IRepetierEvent
    {
        public PrinterState PrinterState { get; set; }
    }
}
