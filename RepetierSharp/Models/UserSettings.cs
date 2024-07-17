using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace RepetierSharp.Models
{
    public class UserSettings
    {
        [JsonPropertyName("gcodeGroup")] public string GCodeGroup { get; set; }

        [JsonPropertyName("gcodeSortBy")] public string GCodeSortedBy { get; set; }

        [JsonPropertyName("gcodeViewMode")] public string GCodeViewMode { get; set; }

        [JsonPropertyName("tempDiagActive")] public string TempDiagActive { get; set; }

        [JsonPropertyName("tempDiagAll")] public string TempDiagAll { get; set; }

        [JsonPropertyName("tempDiagBed")] public string TempDiagBed { get; set; }

        [JsonPropertyName("tempDiagChamber")] public string TempDiagChamber { get; set; }

        [JsonPropertyName("tempDiagMode")] public string TempDiagMode { get; set; }

        [JsonPropertyName("theme")] public string Theme { get; set; }
        
        [JsonPropertyName("preview2dOptions")] public string Preview2DOptions { get; set; }
        
        [JsonPropertyName("timelapseViewFailed")] public string TimelapseViewFailed { get; set; }
        
        [JsonPropertyName("timelapseViewMode")] public string TimelapseViewMode { get; set; }
        
        [JsonPropertyName("printerOrder")] public string PrinterOrder { get; set; }
        
        [JsonPropertyName("leftPrinterList")] public string LeftPrinterList { get; set; }
    }
}
