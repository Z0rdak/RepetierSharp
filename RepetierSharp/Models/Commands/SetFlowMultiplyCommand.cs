using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Commands
{
    public class SetFlowMultiplyCommand : IRepetierCommand
    {
        public SetFlowMultiplyCommand(int flowMultiplier)
        {
            FlowMultiplierInPercent = flowMultiplier;
        }

        [JsonPropertyName("speed")] public int FlowMultiplierInPercent { get; set; }

        [JsonIgnore] public string CommandIdentifier => CommandConstants.SET_FLOW_MULTIPLY;
    }
}
