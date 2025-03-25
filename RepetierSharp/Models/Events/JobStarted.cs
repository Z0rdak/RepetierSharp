using System.Text.Json.Serialization;
using RepetierSharp.Models.Communication;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Events
{
    [EventId(EventConstants.JOB_STARTED)]
    public class JobStarted : IEventData
    {
        [JsonPropertyName("start")] public long StartTime { get; set; }
    }
}
