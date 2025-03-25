using System.Text.Json.Serialization;
using RepetierSharp.Models.Communication;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Commands
{
    public class SetMqttConfigCommand(MQTTConfiguration mqttConfig) : ICommandData
    {
        public MQTTConfiguration MqttConfig { get; set; } = mqttConfig;
        [JsonIgnore] public string Action => CommandConstants.SET_MQTT_CONFIG;
    }
}
