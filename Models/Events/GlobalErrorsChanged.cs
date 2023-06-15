using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Events
{
    [EventId("globalErrorsChanged")]
    public class GlobalErrorsChanged : IRepetierEvent
    {
        public GlobalErrorsChanged() { }
    }

}
