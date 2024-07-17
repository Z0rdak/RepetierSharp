using System.Text.Json.Serialization;

namespace RepetierSharp.Config
{
    public class General
    {
        [JsonPropertyName("active")] public bool Active { get; set; }

        [JsonPropertyName("defaultVolumetric")]
        public bool DefaultVolumetric { get; set; }

        [JsonPropertyName("doorHandling")] public int DoorHandling { get; set; }

        [JsonPropertyName("eepromType")] public string EepromType { get; set; }

        [JsonPropertyName("firmwareName")] public string FirmwareName { get; set; }

        [JsonPropertyName("g9091OverrideE")] public bool G9091OverrideE { get; set; }

        [JsonPropertyName("heatedBed")] public bool HeatedBed { get; set; }

        [JsonPropertyName("logHistory")] public bool LogHistory { get; set; }

        [JsonPropertyName("manufacturer")] public string Manufacturer { get; set; }

        [JsonPropertyName("model")] public string Model { get; set; }

        [JsonPropertyName("name")] public string Name { get; set; }

        [JsonPropertyName("numFans")] public int NumFans { get; set; }

        [JsonPropertyName("pauseHandling")] public int PauseHandling { get; set; }

        [JsonPropertyName("pauseSeconds")] public int PauseSeconds { get; set; }

        [JsonPropertyName("printerHomepage")] public string PrinterHomepage { get; set; }

        [JsonPropertyName("printerManual")] public string PrinterManual { get; set; }

        [JsonPropertyName("printerVariant")] public string PrinterVariant { get; set; }

        [JsonPropertyName("sdcard")] public bool Sdcard { get; set; }

        [JsonPropertyName("slug")] public string Slug { get; set; }

        [JsonPropertyName("softwareLight")] public bool SoftwareLight { get; set; }

        [JsonPropertyName("softwarePower")] public bool SoftwarePower { get; set; }

        [JsonPropertyName("tempUpdateEvery")] public int TempUpdateEvery { get; set; }

        [JsonPropertyName("useModelFromSlug")] public string UseModelFromSlug { get; set; }

        [JsonPropertyName("useOwnModelRepository")]
        public bool UseOwnModelRepository { get; set; }
    }
}
