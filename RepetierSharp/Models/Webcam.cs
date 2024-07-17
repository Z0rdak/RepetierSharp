using System.Text.Json.Serialization;

namespace RepetierSharp.Models
{
    public class WebcamState
    {
        [JsonPropertyName("rec")] public bool Recording { get; set; }
    }
}
