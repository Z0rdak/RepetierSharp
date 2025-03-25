using System.Text.Json.Serialization;
using RepetierSharp.Internal;
using RepetierSharp.Models.Communication;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Commands
{
    [CommandId(CommandConstants.SET_FLOW_MULTIPLY)]
    public class SetFlowMultiplyCommand(int flowMultiplier) : ICommandData
    {
        [JsonPropertyName("speed")] public int FlowMultiplierInPercent { get; set; } = flowMultiplier;

        [JsonIgnore] public string Action => CommandConstants.SET_FLOW_MULTIPLY;
    }
}
