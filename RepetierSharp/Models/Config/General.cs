using System.Text.Json.Serialization;

namespace RepetierSharp.Config
{
    public class General
    {
        [JsonPropertyName("active")]
        public bool Active { get; set; }

        [JsonPropertyName("eepromType")]
        public string EepromType { get; set; }

        [JsonPropertyName("fan")]
        public bool Fan { get; set; }

        [JsonPropertyName("firmwareName")]
        public string FirmwareName { get; set; }

        [JsonPropertyName("heatedBed")]
        public bool HeatedBed { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("printerVariant")]
        public string PrinterVariant { get; set; }

        [JsonPropertyName("sdcard")]
        public bool Sdcard { get; set; }

        [JsonPropertyName("slug")]
        public string Slug { get; set; }

        [JsonPropertyName("softwarePower")]
        public bool SoftwarePower { get; set; }

        [JsonPropertyName("tempUpdateEvery")]
        public int TempUpdateEvery { get; set; }
    }

}
