using System.Text.Json.Serialization;
using RepetierSharp.Models.Communication;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Events
{
    [EventId(EventConstants.DISPATCHER_COUNT)]
    public class DispatcherCount : IEventData
    {
        [JsonPropertyName("count")] public int Count { get; set; }
    }
}
