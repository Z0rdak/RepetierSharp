using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Events
{
    public class DuetDialogOpened : IRepetierEvent
    {
        [JsonPropertyName("message")] public string Message { get; set; }
        [JsonPropertyName("mode")] public int Mode { get; set; }
        [JsonPropertyName("seq")] public int Seq { get; set; }
        [JsonPropertyName("timeout")] public int Timeout { get; set; }
        [JsonPropertyName("title")] public string Title { get; set; }
        [JsonPropertyName("dialogId")] public int DialogId { get; set; }
    }
}
