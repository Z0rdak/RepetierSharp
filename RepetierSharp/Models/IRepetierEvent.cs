using System.Text.Json.Serialization;

namespace RepetierSharp.Models
{
    public interface IRepetierMessage
    {
    }

    public interface IRepetierResponse : IRepetierMessage
    {
    }

    public interface IRepetierEvent : IRepetierMessage
    {
    }
    
    public class EmptyEvent : IRepetierEvent {}

    public class PrinterEvent : EmptyEvent
    {
        [JsonPropertyName("slug")] public string Slug { get; set; }
    }
}
