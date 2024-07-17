using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Events
{
    [EventId("layerChanged")]
    public class LayerChanged : IRepetierEvent
    {
        [JsonPropertyName("layer")] public int Layer { get; set; }
        [JsonPropertyName("maxLayer")] public int MaxLayer { get; set; }
    }
}
