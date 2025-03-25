using System.Text.Json.Serialization;
using RepetierSharp.Models.Communication;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Events
{
    [EventId(EventConstants.PRINTER_SETTING_CHANGED)]
    public class PrinterSettingChanged : IEventData
    {
        [JsonPropertyName("key")] public string Key { get; set; }

        [JsonPropertyName("value")] public string Value { get; set; }
    }
}
