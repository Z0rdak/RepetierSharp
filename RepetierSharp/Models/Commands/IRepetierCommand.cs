using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Commands
{
    public interface IRepetierCommand
    {
        [JsonIgnore] public string CommandIdentifier { get; }
    }
}
