using System.Text.Json.Serialization;

namespace RepetierMqtt.Config
{
    public class Connection
    {
        [JsonPropertyName("serial")]
        public SerialConnection Serial { get; }
    }

}
