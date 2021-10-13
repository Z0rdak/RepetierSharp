using System.Text.Json.Serialization;

namespace RepetierMqtt.Models.Commands
{
    public class SetSpeedMultiplyCommand : ICommandData
    {
        [JsonPropertyName("speed")]
        public int SpeedMultiplierInPercent { get; set; }

        public string CommandIdentifier => CommandConstants.SET_SPEED_MULTIPLY;

        public SetSpeedMultiplyCommand(int speedMultiplier)
        {
            SpeedMultiplierInPercent = speedMultiplier;
        }
    }
}
