using System.Text.Json.Serialization;

namespace RepetierMqtt.Models
{
    public class UserSettings
    {
        [JsonPropertyName("gcodeGroup")]
        public string GCodeGroup { get; }

        [JsonPropertyName("gcodeSortBy")]
        public string GCodeSortedBy { get; }

        [JsonPropertyName("gcodeViewMode")]
        public string GCodeViewMode { get; }

        [JsonPropertyName("tempDiagActive")]
        public string TempDiagActive { get; }

        [JsonPropertyName("tempDiagAll")]
        public string TempDiagAll { get; }

        [JsonPropertyName("tempDiagBed")]
        public string TempDiagBed { get; }

        [JsonPropertyName("tempDiagChamber")]
        public string TempDiagChamber { get; }

        [JsonPropertyName("tempDiagMode")]
        public string TempDiagMode { get; }

        [JsonPropertyName("theme")]
        public string Theme { get; }

        public UserSettings() { }
    }
}