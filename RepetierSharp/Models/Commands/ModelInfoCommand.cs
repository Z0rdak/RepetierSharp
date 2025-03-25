using System.Text.Json.Serialization;
using RepetierSharp.Internal;
using RepetierSharp.Models.Communication;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Commands
{
    [CommandId(CommandConstants.MODEL_INFO)]
    public class ModelInfoCommand(int modelId) : ICommandData
    {
        [JsonPropertyName("id")] public int Id { get; } = modelId;

        [JsonIgnore] public string Action => CommandConstants.MODEL_INFO;
    }
}
