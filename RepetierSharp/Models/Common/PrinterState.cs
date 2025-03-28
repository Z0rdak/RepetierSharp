﻿using System.Collections.Generic;
using System.Text.Json.Serialization;
using RepetierSharp.Models.Common;
using RepetierSharp.Models.Communication;

namespace RepetierSharp.Models
{
    public class PrinterState : IEventData
    {
        [JsonPropertyName("activeExtruder")] public int ActiveExtruder { get; set; }

        [JsonPropertyName("autostartNextPrint")]
        public bool AutostartNextPrint { get; set; }

        [JsonPropertyName("debugLevel")] public int DebugLevel { get; set; }

        [JsonPropertyName("doorOpen")] public bool IsDoorOpen { get; set; }

        [JsonPropertyName("extruder")] public List<TemperatureEntry> Extruders { get; set; }

        [JsonPropertyName("fans")] public List<Fan> Fans { get; set; }
        [JsonPropertyName("f")] public double F { get; set; }

        [JsonPropertyName("filterFan")] public bool FilterFan { get; set; }

        [JsonPropertyName("firmware")] public string Firmware { get; set; }

        [JsonPropertyName("firmwareURL")] public string FirmwareUrl { get; set; }

        [JsonPropertyName("flowMultiply")] public double FlowMultiplier { get; set; }

        [JsonPropertyName("hasXHome")] public bool HasXHome { get; set; }

        [JsonPropertyName("hasYHome")] public bool HasYHome { get; set; }

        [JsonPropertyName("hasZHome")] public bool HasZHome { get; set; }

        [JsonPropertyName("heatedBeds")] public List<TemperatureEntry> Heatedbeds { get; set; }

        [JsonPropertyName("heatedChambers")] public List<TemperatureEntry> HeatedChambers { get; set; }

        [JsonPropertyName("layer")] public int CurrentLayer { get; set; }

        [JsonPropertyName("lights")] public int Lights { get; set; }

        [JsonPropertyName("notification")] public string Notification { get; set; }

        [JsonPropertyName("numExtruder")] public int ExtruderCount { get; set; }

        [JsonPropertyName("powerOn")] public bool IsPowerOn { get; set; }

        [JsonPropertyName("rec")] public bool IsRecording { get; set; }

        [JsonPropertyName("sdcardMounted")] public bool IsSDCardMounted { get; set; }

        [JsonPropertyName("shutdownAfterPrint")]
        public bool ShutDownAfterPrint { get; set; }

        [JsonPropertyName("speedMultiply")] public double SpeedMultiplier { get; set; }

        [JsonPropertyName("volumetric")] public bool IsVolumetric { get; set; }
        [JsonPropertyName("webcams")] public List<WebcamState> Webcams { get; set; }

        [JsonPropertyName("x")] public double X { get; set; }

        [JsonPropertyName("y")] public double Y { get; set; }

        [JsonPropertyName("z")] public double Z { get; set; }
    }
}
