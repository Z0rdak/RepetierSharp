using System.Text.Json.Serialization;
using RepetierSharp.Internal;
using RepetierSharp.Models.Commands;
using RepetierSharp.Models.Communication;

namespace RepetierSharp.Models.Responses
{
    [CommandId("getMQTTConfig", "setMQTTConfig")]
    public class MqttConfigResponse : IResponseData
    {
        [JsonPropertyName("mqtt")] public MQTTConfiguration MQTTConfiguration { get; set; }
        [JsonPropertyName("ok")] public bool Ok { get; set; }
    }

}
