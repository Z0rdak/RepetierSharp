using System.Text.Json.Serialization;

namespace RepetierMqtt.Config
{
    public class SerialConnection
    {
        [JsonPropertyName("baudrate")]
        public int Baudrate { get; }

        [JsonPropertyName("device")]
        public string Device { get; }

        [JsonPropertyName("inputBufferSize")]
        public int InputBufferSize { get; }

        [JsonPropertyName("pingPong")]
        public bool PingPong { get; }

        [JsonPropertyName("protocol")]
        public int Protocol { get; }
    }

}
