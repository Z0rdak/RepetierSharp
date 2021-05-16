using RepetierMqtt.Models;
using System.Text.Json.Serialization;

namespace RepetierMqtt.Models.Events
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