using System.Text.Json.Serialization;
using RepetierSharp.Internal;
using RepetierSharp.Models.Communication;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Events
{
    [EventId(EventConstants.FIRMWARE_CHANGED)]
    [CommandId(CommandConstants.GET_FIRMWARE_DATA)]
    public class FirmwareData : IEventData, IResponseData
    {
        [JsonPropertyName("firmware")] public FirmwareInfo Firmware { get; set; }
    }

    public class FirmwareInfo
    {
        [JsonPropertyName("eeprom")] public string Eeprom { get; set; }

        [JsonPropertyName("firmware")] public string Firmware { get; set; }

        [JsonPropertyName("firmwareURL")] public string FirmwareURL { get; set; }

        [JsonPropertyName("name")] public string Name { get; set; }
    }
}
