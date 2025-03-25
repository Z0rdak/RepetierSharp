using System.Text.Json.Serialization;
using RepetierSharp.Models.Communication;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Events
{
    [EventId(EventConstants.JOB_KILLED, EventConstants.JOB_DEACTIVATED, EventConstants.JOB_FINISHED)]
    public class JobState : IEventData
    {
        [JsonPropertyName("start")] public long StartTime { get; set; }

        [JsonPropertyName("duration")] public uint Duration { get; set; }

        [JsonPropertyName("end")] public long EndTime { get; set; }

        [JsonPropertyName("lines")] public uint PrintedLines { get; set; }
    }
}
