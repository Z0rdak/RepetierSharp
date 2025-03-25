using System.Text.Json.Serialization;
using RepetierSharp.Models.Communication;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Events
{
    [EventId(EventConstants.GCODE_STORAGE_CHANGED)]
    public class GcodeStorageChange : IEventData
    {
        [JsonPropertyName("slug")] public string Slug {get; set; }
        [JsonPropertyName("storage")]  public string Storage {get; set; }
    }
}
