using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Events
{
    public class LayerChangedEvent : IRepetierEvent
    {
        [JsonPropertyName("layer")]
        public int Layer { get; set; }

        public LayerChangedEvent() { }
    }
}