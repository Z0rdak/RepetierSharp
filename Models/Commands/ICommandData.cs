using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Commands
{
    public interface ICommandData
    {
        [JsonIgnore]
        public string CommandIdentifier { get; }
    }
}
