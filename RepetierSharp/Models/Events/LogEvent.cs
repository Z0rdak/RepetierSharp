using RepetierSharp.Models;
using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Events
{
    public class LogEvent : IRepetierEvent
    {
        [JsonPropertyName("id")]
        public int Id { get; }

        [JsonPropertyName("time")]
        public long Timestamp { get; }

        [JsonPropertyName("text")]
        public string Message { get; }

        [JsonPropertyName("type")]
        public LogType LogType { get; }

        public LogEvent() { }
    }

}