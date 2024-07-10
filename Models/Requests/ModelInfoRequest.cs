using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Requests
{
    public class ModelInfoRequest : IRepetierRequest
    {
        public ModelInfoRequest(int modelId)
        {
            Id = modelId;
        }

        [JsonPropertyName("id")] public int Id { get; }

        [JsonIgnore] public string CommandIdentifier => CommandConstants.MODEL_INFO;
    }
}
