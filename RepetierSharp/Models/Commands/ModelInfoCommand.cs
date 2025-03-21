using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Commands
{
    public class ModelInfoCommand : ICommandData
    {
        public ModelInfoCommand(int modelId)
        {
            Id = modelId;
        }

        [JsonPropertyName("id")] public int Id { get; }

        [JsonIgnore] public string Action => CommandConstants.MODEL_INFO;
    }
}
