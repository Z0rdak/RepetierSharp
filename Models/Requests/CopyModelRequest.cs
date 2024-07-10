using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Requests
{
    public class CopyModelRequest : IRepetierRequest
    {
        public CopyModelRequest(int modelId, bool autostart = true)
        {
            Id = modelId;
            Autostart = autostart;
        }

        [JsonPropertyName("id")] public int Id { get; }

        [JsonPropertyName("autostart")] public bool Autostart { get; }

        [JsonIgnore] public string CommandIdentifier => CommandConstants.COPY_MODEL;
    }
}
