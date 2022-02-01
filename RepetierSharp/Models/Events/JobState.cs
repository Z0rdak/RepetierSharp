using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Events
{
    [EventId("jobKilled", "jobDeactivated", "jobFinished")]
    public class JobState : IRepetierEvent
    {
        [JsonPropertyName("start")]
        public long StartTime { get; set; }

        [JsonPropertyName("duration")]
        public uint Duration { get; set; }

        [JsonPropertyName("end")]
        public long EndTime { get; set; }

        [JsonPropertyName("lines")]
        public uint PrintedLines { get; set; }

        public JobState() { }
    }
}