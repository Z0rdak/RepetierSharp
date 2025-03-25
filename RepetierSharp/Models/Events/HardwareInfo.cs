using System.Collections.Generic;
using System.Text.Json.Serialization;
using RepetierSharp.Models.Communication;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Events
{
    [EventId(EventConstants.HARDWARE_INFO)]
    public class HardwareInfo : IEventData
    {
        [JsonPropertyName("list")]
        public List<HardwareInfoEntry> HardwareInfos { get; set; }

        [JsonPropertyName("maxUrgency")]
        public int MaxUrgency { get; set; }
    }

    public class HardwareInfoEntry
    {
        [JsonPropertyName("name")] public string Name { get; set; }
        [JsonPropertyName("msgType")] public int MsgType { get; set; }
        [JsonPropertyName("unit")] public string Unit { get; set; }
        [JsonPropertyName("text")] public string Text { get; set; }
        [JsonPropertyName("value")] public double Value { get; set; }
        [JsonPropertyName("urgency")] public int Urgency { get; set; }
        [JsonPropertyName("icon")] public int Icon { get; set; }
        [JsonPropertyName("url")] public string Url { get; set; }
    }

}
