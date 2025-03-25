using System.Text.Json.Serialization;
using RepetierSharp.Models.Communication;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Commands
{
    public class GetMqttConfigCommand : ICommandData
    {
        public static GetMqttConfigCommand Instance => new();
        [JsonIgnore] public string Action => CommandConstants.GET_MQTT_CONFIG;
    }
}
