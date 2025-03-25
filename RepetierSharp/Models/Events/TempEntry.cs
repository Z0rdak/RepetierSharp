using System.Text.Json.Serialization;
using RepetierSharp.Models.Communication;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Events
{
    [EventId(EventConstants.TEMP)]
    public class TempEntry : IEventData
    {
        [JsonPropertyName("O")] public double Output { get; set; }

        [JsonPropertyName("S")] public double Set { get; set; }

        [JsonPropertyName("T")] public double Measured { get; set; }

        [JsonPropertyName("id")] public int Id { get; set; }

        [JsonPropertyName("t")] public long Timestamp { get; set; }
    }
}
