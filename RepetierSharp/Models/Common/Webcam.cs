using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Common
{
    public class WebcamState
    {
        [JsonPropertyName("rec")] public bool Recording { get; set; }
    }
}
