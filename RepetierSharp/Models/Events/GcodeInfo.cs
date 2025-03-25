using System.Text.Json.Serialization;
using RepetierSharp.Models.Communication;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Events
{
    [EventId(EventConstants.GCODE_INFO_UPDATED, EventConstants.GCODE_ANALYSIS_FINISHED)]
    public class GcodeInfo : PrinterEventData
    {
        [JsonPropertyName("modelId")] public int ModelId { get; set; }
        
        [JsonPropertyName("modelPath")] public string ModelPath { get; set; }
        
        [JsonPropertyName("list")] public string List { get; set; }
    }
}
