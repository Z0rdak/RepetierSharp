using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Events
{
    [EventId(EventConstants.PROJECT_FOLDER_CHANGED)]
    public class ProjectFolderChanged
    {
        [JsonPropertyName("idx")] public int Idx { get; set; }
        [JsonPropertyName("server_uuid")] public string ServerUuid { get; set; }
        [JsonPropertyName("version")] public int Version { get; set; }
    }
}
