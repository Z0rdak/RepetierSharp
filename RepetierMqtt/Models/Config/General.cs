using System.Text.Json.Serialization;

namespace RepetierMqtt.Config
{
    public class General
    {
        [JsonPropertyName("active")]
        public bool Active { get; }

        [JsonPropertyName("eepromType")]
        public string EepromType { get; }

        [JsonPropertyName("fan")]
        public bool Fan { get; }

        [JsonPropertyName("firmwareName")]
        public string FirmwareName { get; }

        [JsonPropertyName("heatedBed")]
        public bool HeatedBed { get; }

        [JsonPropertyName("name")]
        public string Name { get; }

        [JsonPropertyName("printerVariant")]
        public string PrinterVariant { get; }

        [JsonPropertyName("sdcard")]
        public bool Sdcard { get; }

        [JsonPropertyName("slug")]
        public string Slug { get; }

        [JsonPropertyName("softwarePower")]
        public bool SoftwarePower { get; }

        [JsonPropertyName("tempUpdateEvery")]
        public int TempUpdateEvery { get; }
    }

}
