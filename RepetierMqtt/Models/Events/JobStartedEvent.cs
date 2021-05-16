using System.Text.Json.Serialization;

namespace RepetierMqtt.Models.Events
{
    public class JobStartedEvent : IRepetierEvent
    {
        [JsonPropertyName("start")]
        public long StartTime { get; }

        public JobStartedEvent() { }
    }
}