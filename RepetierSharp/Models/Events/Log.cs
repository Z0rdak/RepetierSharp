using RepetierSharp.Models;
using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Events
{
    [EventId("log")]
    public class Log : IRepetierEvent
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("time")]
        public long Timestamp { get; set; }

        [JsonPropertyName("text")]
        public string Message { get; set; }

        [JsonPropertyName("type")]
        public LogType LogType { get; set; }

        public Log() { }
    }

}