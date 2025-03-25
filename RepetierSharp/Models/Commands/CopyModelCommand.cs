using System.Text.Json.Serialization;
using RepetierSharp.Internal;
using RepetierSharp.Models.Communication;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Commands
{
    [CommandId(CommandConstants.COPY_MODEL)]
    public class CopyModelCommand(int modelId, bool autostart = true) : ICommandData
    {
        [JsonPropertyName("id")] public int Id { get; } = modelId;

        [JsonPropertyName("autostart")] public bool Autostart { get; } = autostart;

        [JsonIgnore] public string Action => CommandConstants.COPY_MODEL;
    }
}
