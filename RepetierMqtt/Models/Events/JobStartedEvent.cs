using System.Text.Json.Serialization;

namespace RepetierMqtt.Event
{
    public class JobStartedEvent : IRepetierEvent
    {
        [JsonPropertyName("start")]
        public long StartTime { get; }

        public JobStartedEvent() { }
    }
}