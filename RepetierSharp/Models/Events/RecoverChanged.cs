using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Events
{
    [EventId("recoverChanged")]
    public class RecoverChanged : IRepetierEvent
    {
        [JsonPropertyName("state")]
        public int State { get; }
    }
}
