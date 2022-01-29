using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Commands
{
    public class SetSpeedMultiplyCommand : ICommandData
    {
        [JsonPropertyName("speed")]
        public int SpeedMultiplierInPercent { get; set; }
        [JsonIgnore]
        public string CommandIdentifier => CommandConstants.SET_SPEED_MULTIPLY;

        public SetSpeedMultiplyCommand(int speedMultiplier)
        {
            SpeedMultiplierInPercent = speedMultiplier;
        }
    }
}
