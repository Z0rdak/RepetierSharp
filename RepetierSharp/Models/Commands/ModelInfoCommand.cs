using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Commands
{
    public class ModelInfoCommand : IRepetierCommand
    {
        public ModelInfoCommand(int modelId)
        {
            Id = modelId;
        }

        [JsonPropertyName("id")] public int Id { get; }

        [JsonIgnore] public string CommandIdentifier => CommandConstants.MODEL_INFO;
    }
}
