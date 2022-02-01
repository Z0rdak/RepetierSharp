using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Events
{
    public class JobFinishedEvent : IRepetierEvent
    {
        [JsonPropertyName("start")]
        public long StartTime { get; set; }

        [JsonPropertyName("duration")]
        public uint Duration { get; set; }

        [JsonPropertyName("end")]
        public long EndTime { get; set; }

        [JsonPropertyName("lines")]
        public uint PrintedLines { get; set; }

        public JobFinishedEvent() { }
    }
}