using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Communication
{
    public interface ICommandData
    {
        [JsonIgnore] public string Action { get; }
    }
}
