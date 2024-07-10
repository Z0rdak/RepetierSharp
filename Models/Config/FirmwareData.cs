using System.Text.Json.Serialization;
using RepetierSharp.Models.Events;

namespace RepetierSharp.Models.Config
{
    public class FirmwareData : IRepetierEvent
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
