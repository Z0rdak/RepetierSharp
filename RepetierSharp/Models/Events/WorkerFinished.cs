using System.Text.Json.Serialization;
using RepetierSharp.Models.Communication;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Events
{
    [EventId(EventConstants.WORKER_FINISHED)]
    public class WorkerFinished : IEventData
    {
        [JsonPropertyName("message")] public string Message { get; set; }
        
        [JsonPropertyName("id")] public int Id { get; set; }
        
        [JsonPropertyName("type")] public string Type { get; set; }
    }

}
