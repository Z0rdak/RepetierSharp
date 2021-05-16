using RepetierMqtt.Models;
using RepetierMqtt.Models.Commands;
using System.Text.Json.Serialization;

namespace RepetierMqtt
{
    public class ModelInfoCommand : ICommandData
    {
        [JsonPropertyName("id")]
        public int Id { get; }

        public string CommandIdentifier => CommandConstants.MODEL_INFO;

        public ModelInfoCommand(int modelId)
        {
            this.Id = modelId;
        }
    }
}