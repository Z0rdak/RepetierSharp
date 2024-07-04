using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Requests
{
    public class SetFanSpeedRequest : IRepetierRequest
    {
        [JsonPropertyName("speed")]
        public int FanSpeed { get; set; }
        [JsonPropertyName("fanId")]
        public int FanId { get; set; }

        public const int MAX_THROTTLE = 255;
        public const int FAN_OFF = 0;
        [JsonIgnore]
        public string CommandIdentifier => CommandConstants.SET_FAN_SPEED;

        public SetFanSpeedRequest(int fanSpeed, int fanId = 0)
        {
            FanSpeed = fanSpeed < FAN_OFF ? FAN_OFF : fanSpeed;
            FanSpeed = fanSpeed > MAX_THROTTLE ? MAX_THROTTLE : fanSpeed;
            FanId = fanId < 0 ? 0 : fanId;
        }
    }
}
