using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Events
{
    [EventId("printerConditionChanged")]
    public class PrinterConditionChanged : IRepetierEvent
    {
        [JsonPropertyName("condition")]
        public PrinterCondition Condition { get; set; }

        // line causing the change if it is an error
        [JsonPropertyName("reason")]
        public string Reason { get; set; }
        
        public PrinterConditionChanged() { }
    }

    public enum PrinterCondition
    {
        Unknown = 0,  // After start when it is unknown
        Ready = 1,    // Should accept commands
        Shutdown = 2, // Connection to server but printer is down
        Killed = 3,   // Printer killed it self
        Ignoring = 4, // E.g. after temp. error when M999 is required
        Offline = 5   // Printer was never connected
        
    }

}
