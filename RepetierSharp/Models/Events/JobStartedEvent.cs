using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Events
{
    public class JobStartedEvent : IRepetierEvent
    {
        [JsonPropertyName("start")]
        public long StartTime { get; set; }

        public JobStartedEvent() { }
    }
}