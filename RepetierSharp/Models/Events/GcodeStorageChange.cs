using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Events
{
    [EventId("gcodestoragechanged")]
    public class GcodeStorageChange : IRepetierEvent
    {
        [JsonPropertyName("slug")] public string Slug {get; set; }
        [JsonPropertyName("storage")]  public string Storage {get;}
    }
}
