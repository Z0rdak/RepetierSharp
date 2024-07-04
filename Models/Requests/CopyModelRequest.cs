using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Requests
{
    public class CopyModelRequest : IRepetierRequest
    {
        [JsonIgnore]
        public string CommandIdentifier => CommandConstants.COPY_MODEL;

        [JsonPropertyName("id")]
        public int Id { get; }

        [JsonPropertyName("autostart")]
        public bool Autostart { get; }

        public CopyModelRequest(int modelId, bool autostart = true)
        {
            this.Id = modelId;
            this.Autostart = autostart;
        }
    }
}
