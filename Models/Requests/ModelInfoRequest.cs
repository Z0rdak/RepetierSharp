using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Requests
{
    public class ModelInfoRequest : IRepetierRequest
    {
        [JsonPropertyName("id")]
        public int Id { get; }
        [JsonIgnore]
        public string CommandIdentifier => CommandConstants.MODEL_INFO;

        public ModelInfoRequest(int modelId)
        {
            this.Id = modelId;
        }
    }
}
