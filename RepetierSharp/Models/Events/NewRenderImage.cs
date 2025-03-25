using System.Text.Json.Serialization;
using RepetierSharp.Models.Communication;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Events
{
    [EventId(EventConstants.NEW_RENDER_IMAGE)]
    public class NewRenderImage : PrinterEventData
    {
        [JsonPropertyName("id")] public int Id { get; set; }
        
        [JsonPropertyName("list")] public string List { get; set; }
    }
}
