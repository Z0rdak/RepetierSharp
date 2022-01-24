using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Events
{
    public class JobKilledEvent : IRepetierEvent
    {
        [JsonPropertyName("start")]
        public long StartTime { get; }

        [JsonPropertyName("duration")]
        public uint Duration { get; }

        [JsonPropertyName("end")]
        public long EndTime { get; }

        [JsonPropertyName("lines")]
        public uint PrintedLines { get; }

        public JobKilledEvent() { }
    }
}