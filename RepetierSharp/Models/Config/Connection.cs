using System.Text.Json.Serialization;

namespace RepetierSharp.Config
{
    public class Connection
    {
        [JsonPropertyName("serial")]
        public SerialConnection Serial { get; set; }
    }

}
