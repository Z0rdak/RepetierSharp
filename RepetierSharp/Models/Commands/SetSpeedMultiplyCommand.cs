using System.Text.Json.Serialization;
using RepetierSharp.Internal;
using RepetierSharp.Models.Communication;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Commands
{
    [CommandId(CommandConstants.SET_SPEED_MULTIPLY)]
    public class SetSpeedMultiplyCommand(int speedMultiplier) : ICommandData
    {
        [JsonPropertyName("speed")] public int SpeedMultiplierInPercent { get; set; } = speedMultiplier;

        [JsonIgnore] public string Action => CommandConstants.SET_SPEED_MULTIPLY;
    }
}
