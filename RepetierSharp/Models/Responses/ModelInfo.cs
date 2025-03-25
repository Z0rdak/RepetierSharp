using System.Collections.Generic;
using System.Text.Json.Serialization;
using RepetierSharp.Models.Communication;

namespace RepetierSharp.Models.Responses
{
    /// <summary>
    ///     Represents a g-code file stored in the printer. It also contains some statistical data.
    /// </summary>
    public class ModelInfo : IResponseData
    {
        [JsonPropertyName("analysed")] public PrintJobAnalyzed Analysed { get; set; }
        [JsonPropertyName("created")] public long Created { get; set; }
        [JsonPropertyName("filamentTotal")] public double FilamentTotal { get; set; }
        [JsonPropertyName("fits")] public bool Fits { get; set; }
        [JsonPropertyName("gcodePatch")] public string GCodePatch { get; set; }
        [JsonPropertyName("group")] public string Group { get; set; }
        [JsonPropertyName("id")] public int Id { get; set; }
        [JsonPropertyName("lastPrintTime")] public double LastPrintTime { get; set; }
        [JsonPropertyName("layer")] public int Layer { get; set; }
        [JsonPropertyName("length")] public long Length { get; set; }
        [JsonPropertyName("lines")] public int Lines { get; set; }
        [JsonPropertyName("extruderUsage")] public List<double> ExtruderUsage { get; set; } = new ();
        [JsonPropertyName("materials")] public List<string> Materials { get; set; } = new ();
        [JsonPropertyName("volumeUsage")] public List<double> VolumeUsage { get; set; } = new ();
        [JsonPropertyName("name")] public string Name { get; set; }
        [JsonPropertyName("repeat")] public int Repeat { get; set; }
        [JsonPropertyName("notes")] public string Notes { get; set; }
        [JsonPropertyName("printTime")] public double PrintTime { get; set; }
        [JsonPropertyName("printed")] public int Printed { get; set; }
        [JsonPropertyName("radius")] public double Radius { get; set; }
        [JsonPropertyName("printerType")] public double PrinterType { get; set; }
        [JsonPropertyName("printerParam1")] public double PrinterParam1 { get; set; }
        [JsonPropertyName("radiusMove")] public double RadiusMove { get; set; }
        [JsonPropertyName("slicer")] public string Slicer { get; set; }
        [JsonPropertyName("state")] public string State { get; set; }
        [JsonPropertyName("version")] public int Version { get; set; }
        [JsonPropertyName("volumeTotal")] public double VolumeTotal { get; set; }
        [JsonPropertyName("volumetric")] public bool Volumetric { get; set; }
        [JsonPropertyName("xMax")] public double XMax { get; set; }
        [JsonPropertyName("xMaxMove")] public double XMaxMove { get; set; }
        [JsonPropertyName("xMaxView")] public double XMaxView { get; set; }
        [JsonPropertyName("xMin")] public double XMin { get; set; }
        [JsonPropertyName("xMinMove")] public double XMinMove { get; set; }
        [JsonPropertyName("xMinView")] public double XMinView { get; set; }
        [JsonPropertyName("yMax")] public double YMax { get; set; }
        [JsonPropertyName("yMaxMove")] public double YMaxMove { get; set; }
        [JsonPropertyName("yMaxView")] public double YMaxView { get; set; }
        [JsonPropertyName("yMin")] public double YMin { get; set; }
        [JsonPropertyName("yMinMove")] public double YMinMove { get; set; }
        [JsonPropertyName("yMinView")] public double YMinView { get; set; }
        [JsonPropertyName("zMax")] public double ZMax { get; set; }
        [JsonPropertyName("zMin")] public double ZMin { get; set; }
    }

    public class ModelInfoList : IResponseData
    {
        [JsonPropertyName("data")]
        public List<ModelInfo> Models { get; set; } = new();
    }
}
