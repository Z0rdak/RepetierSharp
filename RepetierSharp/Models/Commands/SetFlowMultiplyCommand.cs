using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Commands
{
    public class SetFlowMultiplyCommand : ICommandData
    {
        [JsonPropertyName("speed")]
        public int FlowMultiplierInPercent { get; set; }

        public string CommandIdentifier => CommandConstants.SET_FLOW_MULTIPLY;

        public SetFlowMultiplyCommand(int flowMultiplier)
        {
            FlowMultiplierInPercent = flowMultiplier;
        }
    }
}
