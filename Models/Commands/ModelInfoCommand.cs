using RepetierSharp.Models;
using RepetierSharp.Models.Commands;
using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Commands
{
    public class ModelInfoCommand : ICommandData
    {
        [JsonPropertyName("id")]
        public int Id { get; }
        [JsonIgnore]
        public string CommandIdentifier => CommandConstants.MODEL_INFO;

        public ModelInfoCommand(int modelId)
        {
            this.Id = modelId;
        }
    }
}