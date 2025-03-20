using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Events
{
    [EventId("newRenderImage")]
    public class NewRenderImage : PrinterEvent
    {
        [JsonPropertyName("id")]
        public int Id { get; set; }
        
        [JsonPropertyName("list")]
        public string List { get; set; }
    }
}
