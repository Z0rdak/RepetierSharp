using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Events
{
    public class TimelapseChanged
    {
        [JsonPropertyName("running")] 
        public bool Running { get; set; }
        [JsonPropertyName("runningEntries")]
        List<TimeLapseEntry> RunningEntries { get; set; }
        [JsonPropertyName("timelapses")]
        List<TimeLapseEntry> Timelapses { get; set; }
    }


    public class TimeLapseEntry
    {
        [JsonPropertyName("bitrate")] public int Bitrate { get; set; }
        [JsonPropertyName("conversionError")] public string ConversionError { get; set; }
        [JsonPropertyName("conversionMode")] public int ConversionMode { get; set; }
        [JsonPropertyName("createdSeconds")] public int CreatedSeconds { get; set; }
        [JsonPropertyName("dir")] public string Dir { get; set; }
        [JsonPropertyName("framerate")] public int Framerate { get; set; }
        [JsonPropertyName("imageCounter")] public int ImageCounter { get; set; }
        [JsonPropertyName("name")] public string Name { get; set; }
        [JsonPropertyName("valid")] public bool Valid { get; set; }
        [JsonPropertyName("videoLength")] public int VideoLength { get; set; }
        [JsonPropertyName("webcamId")] public int WebcamId { get; set; }
    }
}
