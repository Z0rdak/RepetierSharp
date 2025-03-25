using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Communication
{
    public interface IEventData { }
    public class EmptyEventData : IEventData { }
    public class ServerEventData : IEventData { }
    public class PrinterEventData : IEventData
    {
        [JsonPropertyName("slug")] public string Slug { get; set; }
    }
}
