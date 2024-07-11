using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Commands
{
    public class SetSpeedMultiplyCommand : IRepetierCommand
    {
        public SetSpeedMultiplyCommand(int speedMultiplier)
        {
            SpeedMultiplierInPercent = speedMultiplier;
        }

        [JsonPropertyName("speed")] public int SpeedMultiplierInPercent { get; set; }

        [JsonIgnore] public string CommandIdentifier => CommandConstants.SET_SPEED_MULTIPLY;
    }
}
