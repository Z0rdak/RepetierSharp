using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Commands
{
    public class SetFanSpeedCommand : IRepetierCommand
    {
        public const int MAX_THROTTLE = 255;
        public const int FAN_OFF = 0;

        public SetFanSpeedCommand(int fanSpeed, int fanId = 0)
        {
            FanSpeed = fanSpeed < FAN_OFF ? FAN_OFF : fanSpeed;
            FanSpeed = fanSpeed > MAX_THROTTLE ? MAX_THROTTLE : fanSpeed;
            FanId = fanId < 0 ? 0 : fanId;
        }

        [JsonPropertyName("speed")] public int FanSpeed { get; set; }

        [JsonPropertyName("fanId")] public int FanId { get; set; }

        [JsonIgnore] public string CommandIdentifier => CommandConstants.SET_FAN_SPEED;
    }
}
