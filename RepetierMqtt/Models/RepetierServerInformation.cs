using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RepetierMqtt.Models
{

    /// <summary>
    /// Contains general information regarding the Repetier Server (API-Key, printer names, version, ...)
    /// </summary>
    public class RepetierServerInformation
    {
        [JsonPropertyName("apikey")]
        public string ApiKey { get; }

        [JsonPropertyName("name")]
        public string Name { get; }

        [JsonPropertyName("version")]
        public string Version { get; }

        [JsonPropertyName("printers")]
        public List<PrinterInfo> Printers { get; }

        [JsonPropertyName("servername")]
        public string ServerName { get; }

        [JsonPropertyName("serveruuid")]
        public string ServerUUID { get; }

        public RepetierServerInformation() { }
    }
}