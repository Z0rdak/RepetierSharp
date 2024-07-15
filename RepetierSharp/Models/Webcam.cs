using System.Text.Json.Serialization;

namespace RepetierSharp.Models
{
    public class Webcam
    {
        [JsonPropertyName("rec")] public bool Recording { get; set; }
    }
}
