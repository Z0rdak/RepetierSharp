using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Commands
{
    public class CopyModelCommand : ICommandData
    {
        public CopyModelCommand(int modelId, bool autostart = true)
        {
            Id = modelId;
            Autostart = autostart;
        }

        [JsonPropertyName("id")] public int Id { get; }

        [JsonPropertyName("autostart")] public bool Autostart { get; }

        [JsonIgnore] public string Action => CommandConstants.COPY_MODEL;
    }
}
