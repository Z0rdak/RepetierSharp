using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Events
{
    public class ProjectFolderChanged
    {
        [JsonPropertyName("idx")] public int Idx { get; set; }
        [JsonPropertyName("server_uuid")] public string ServerUuid { get; set; }
        [JsonPropertyName("version")] public int Version { get; set; }
    }
}
