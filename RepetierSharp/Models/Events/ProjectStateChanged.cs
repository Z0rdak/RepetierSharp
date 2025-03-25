using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Events
{
    [EventId(EventConstants.PROJECT_STATE_CHANGED)]
    public class ProjectStateChanged
    {
        [JsonPropertyName("uuid")] public string Uuid { get; set; }
        [JsonPropertyName("version")] public int Version { get; set; }
    }
}
