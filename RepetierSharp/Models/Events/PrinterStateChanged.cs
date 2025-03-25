using RepetierSharp.Models.Communication;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Events
{
    [EventId(EventConstants.STATE)]
    public class PrinterStateChanged : IEventData
    {
        public PrinterState PrinterState { get; set; }
    }
}
