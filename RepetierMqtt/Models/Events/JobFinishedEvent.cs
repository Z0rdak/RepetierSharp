using System.Text.Json.Serialization;

namespace RepetierMqtt.Event
{
    public class JobFinishedEvent : IRepetierEvent
    {
        [JsonPropertyName("start")]
        public long StartTime { get; }

        [JsonPropertyName("duration")]
        public uint Duration { get; }

        [JsonPropertyName("end")]
        public long EndTime { get; }

        [JsonPropertyName("lines")]
        public uint PrintedLines { get; }

        public JobFinishedEvent() { }
    }
}