using System.Text.Json.Serialization;

namespace RepetierSharp.Config
{
    public class SerialConnection
    {
        [JsonPropertyName("baudrate")]
        public int Baudrate { get; set; }

        [JsonPropertyName("device")]
        public string Device { get; set; }

        [JsonPropertyName("inputBufferSize")]
        public int InputBufferSize { get; set; }

        [JsonPropertyName("pingPong")]
        public bool PingPong { get; set; }

        [JsonPropertyName("protocol")]
        public int Protocol { get; set; }
    }

}
