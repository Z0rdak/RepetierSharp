using System.Text.Json.Serialization;
using RepetierSharp.Models.Communication;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Events
{
    [EventId(EventConstants.MQTT_STATE_CHANGED)]
    public class MqttStateChanged : IEventData
    {
        [JsonPropertyName("connected")] public bool Connected { get; set; }
        [JsonPropertyName("lastError")] public string LastError  { get; set; }
    }
}
