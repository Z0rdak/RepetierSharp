using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Events
{
    [EventId("dispatcherCount")]
    public class DispatcherCount : IRepetierEvent
    {
        [JsonPropertyName("count")]
        public int Count { get; set; }
    }
}
