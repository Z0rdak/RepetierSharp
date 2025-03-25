using System.Text.Json.Serialization;
using RepetierSharp.Models.Communication;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Events
{
    [EventId(EventConstants.LAYER_CHANGED)]
    public class LayerChanged : IEventData
    {
        [JsonPropertyName("layer")] public int Layer { get; set; }
        
        [JsonPropertyName("maxLayer")] public int MaxLayer { get; set; }
    }
}
