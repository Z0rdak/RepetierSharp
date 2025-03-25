using System.Text.Json.Serialization;
using RepetierSharp.Models.Communication;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Events
{
    [EventId(EventConstants.LOG)]
    public class LogEntry : IEventData
    {
        [JsonPropertyName("id")] public int Id { get; set; }

        [JsonPropertyName("time")] public long Timestamp { get; set; }

        [JsonPropertyName("text")] public string Message { get; set; }

        [JsonPropertyName("type")] public LogType LogType { get; set; }
    }
}
