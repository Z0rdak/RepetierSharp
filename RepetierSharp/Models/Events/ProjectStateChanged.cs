using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Events
{
    public class ProjectStateChanged
    {
        [JsonPropertyName("uuid")] public string Uuid { get; set; }
        [JsonPropertyName("version")] public int Version { get; set; }
    }
}
