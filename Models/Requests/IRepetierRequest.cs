using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Requests
{
    public interface IRepetierRequest
    {
        [JsonIgnore] public string CommandIdentifier { get; }
    }
}
