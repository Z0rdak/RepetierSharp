using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Events
{
    [EventId("jobStarted")]
    public class JobStarted : IRepetierEvent
    {
        [JsonPropertyName("start")] public long StartTime { get; set; }
    }
}
