using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace RepetierMqtt.Models
{
    public class PrinterState
    {
        [JsonPropertyName("activeExtruder")]
        public int ActiveExtruder { get; }

        [JsonPropertyName("autostartNextPrint")]
        public bool AutostartNextPrint { get; }

        [JsonPropertyName("debugLevel")]
        public int DebugLevel { get; }

        [JsonPropertyName("doorOpen")]
        public bool IsDoorOpen { get; }

        [JsonPropertyName("extruder")]
        public List<PrinterTemperature> Extruders { get; set; }

        [JsonPropertyName("fans")]
        public List<Fan> Fans { get; }

        [JsonPropertyName("filterFan")]
        public bool FilterFan { get; }

        [JsonPropertyName("firmware")]
        public string Firmware { get; }

        [JsonPropertyName("firmwareURL")]
        public string FirmwareUrl { get; }

        [JsonPropertyName("flowMultiply")]
        public double FlowMultiplier { get; }

        [JsonPropertyName("hasXHome")]
        public bool HasXHome { get; }

        [JsonPropertyName("hasYHome")]
        public bool HasYHome { get; }

        [JsonPropertyName("hasZHome")]
        public bool HasZHome { get; }

        [JsonPropertyName("heatedBeds")]
        public List<PrinterTemperature> Heatedbeds { get; }

        [JsonPropertyName("heatedChambers")]
        public List<PrinterTemperature> HeatedChambers { get; }

        [JsonPropertyName("layer")]
        public int CurrentLayer { get; }

        [JsonPropertyName("lights")]
        public int Lights { get; }

        [JsonPropertyName("notification")]
        public string Notification { get; }

        [JsonPropertyName("numExtruder")]
        public int ExtruderCount { get; }

        [JsonPropertyName("powerOn")]
        public bool IsPowerOn { get; }

        [JsonPropertyName("rec")]
        public bool IsRecording { get; }

        [JsonPropertyName("sdcardMounted")]
        public bool IsSDCardMounted { get; }

        [JsonPropertyName("shutdownAfterPrint")]
        public bool ShutDownAfterPrint { get; }

        [JsonPropertyName("speedMultiply")]
        public double SpeedMultiplier { get; }

        [JsonPropertyName("volumetric")]
        public bool IsVolumetric { get; }

        [JsonPropertyName("x")]
        public double X { get; }

        [JsonPropertyName("y")]
        public double Y { get; }

        [JsonPropertyName("z")]
        public double Z { get; }
    }
}