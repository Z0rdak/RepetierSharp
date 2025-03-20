using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Events
{
    [EventId("workerFinished")]
    public class WorkerFinished : IRepetierEvent
    {
        [JsonPropertyName("message")]
        public string Message { get; set; }
        
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [JsonPropertyName("type")]
        public string Type { get; set; }
    }

}
