using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Events
{
    [EventId("gcodeInfoUpdated", "gcodeAnalysisFinished")]
    public class GcodeInfo : PrinterEvent
    {
        [JsonPropertyName("modelId")]
        public int ModelId { get; set; }
        
        [JsonPropertyName("modelPath")]
        public string ModelPath { get; set; }
        
        [JsonPropertyName("list")]
        public string List { get; set; }
    }
}
