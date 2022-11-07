using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace RepetierSharp.Models
{
    public class UserSettings
    {
        [JsonPropertyName("gcodeGroup")]
        public string GCodeGroup { get; set; }

        [JsonPropertyName("gcodeSortBy")]
        public string GCodeSortedBy { get; set; }

        [JsonPropertyName("gcodeViewMode")]
        public string GCodeViewMode { get; set; }

        [JsonPropertyName("tempDiagActive")]
        public string TempDiagActive { get; set; }

        [JsonPropertyName("tempDiagAll")]
        public string TempDiagAll { get; set; }

        [JsonPropertyName("tempDiagBed")]
        public string TempDiagBed { get; set; }

        [JsonPropertyName("tempDiagChamber")]
        public string TempDiagChamber { get; set; }

        [JsonPropertyName("tempDiagMode")]
        public string TempDiagMode { get; set; }

        [JsonPropertyName("theme")]
        public string Theme { get; set; }

        public UserSettings() { }

        public override string ToString()
        {
            return GetType().GetProperties()
                .Select(info => (info.Name, Value: info.GetValue(this, null) ?? "(null)"))
                .Aggregate(
                    new StringBuilder($"{GetType().Name}\n"),
                    (sb, pair) => sb.AppendLine($"- {pair.Name}: {pair.Value}"),
                    sb => sb.ToString());
        }
    }
}
