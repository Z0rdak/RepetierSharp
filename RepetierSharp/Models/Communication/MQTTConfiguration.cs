using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Commands
{
    public class MQTTConfiguration
    {
        [JsonPropertyName("active")]
        public bool Active { get; set; }

        [JsonPropertyName("allowCmdEvents")]
        public bool AllowCmdEvents { get; set; }

        [JsonPropertyName("allowCmdGCode")]
        public bool AllowCmdGCode { get; set; }

        [JsonPropertyName("allowCmdRPC")]
        public bool AllowCmdRPC { get; set; }

        [JsonPropertyName("allowInServerCommands")]
        public bool AllowInServerCommands { get; set; }

        [JsonPropertyName("clientID")]
        public string? ClientID { get; set; }

        [JsonPropertyName("domain")]
        public string Domain { get; set; } 

        [JsonPropertyName("eventExceptions")]
        public List<string> EventExceptions { get; set; }

        [JsonPropertyName("homeassistantDiscoveryPrefix")]
        public string HomeassistantDiscoveryPrefix { get; set; } 

        [JsonPropertyName("max_inflight_messages")]
        public int MaxInflightMessages { get; set; }

        [JsonPropertyName("message_expiry_interval")]
        public int MessageExpiryInterval { get; set; }

        [JsonPropertyName("mqttVersion")]
        public int MqttVersion { get; set; }

        [JsonPropertyName("password")]
        public string Password { get; set; } 

        [JsonPropertyName("port")]
        public int Port { get; set; }

        [JsonPropertyName("retainBasicState")]
        public bool RetainBasicState { get; set; }

        [JsonPropertyName("rootTopic")]
        public string RootTopic { get; set; } 

        [JsonPropertyName("secure")]
        public bool Secure { get; set; }

        [JsonPropertyName("sendEvents")]
        public bool SendEvents { get; set; }

        [JsonPropertyName("sendHomeassistant")]
        public bool SendHomeassistant { get; set; }

        [JsonPropertyName("sendState")]
        public bool SendState { get; set; }

        [JsonPropertyName("session_expiry_interval")]
        public int SessionExpiryInterval { get; set; }

        [JsonPropertyName("user")]
        public string User { get; set; } 

        [JsonPropertyName("validateCertificate")]
        public bool ValidateCertificate { get; set; }
    }
}
