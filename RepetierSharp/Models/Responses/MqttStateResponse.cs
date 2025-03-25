using System.Text.Json.Serialization;
using RepetierSharp.Internal;
using RepetierSharp.Models.Communication;
using RepetierSharp.Models.Events;

namespace RepetierSharp.Models.Responses
{
    [CommandId("getMQTTState")]
    public class MqttStateResponse : IResponseData
    {
        [JsonPropertyName("mqtt")] MqttStateChanged MQTTConfig { get; set; }
        [JsonPropertyName("ok")] public bool Ok { get; set; }
    }
}
