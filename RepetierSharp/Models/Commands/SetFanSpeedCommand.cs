using System.Text.Json.Serialization;
using RepetierSharp.Internal;
using RepetierSharp.Models.Communication;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Commands
{
    [CommandId(CommandConstants.SET_FAN_SPEED)]
    public class SetFanSpeedCommand : ICommandData
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

        [JsonIgnore] public string Action => CommandConstants.SET_FAN_SPEED;
    }
}
