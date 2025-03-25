using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace RepetierSharp.Models
{
    /// <summary>
    ///     Contains general information regarding the Repetier Server (API-Key, printer names, version, ...)
    /// </summary>
    public class RepetierServerInformation
    {
        [JsonPropertyName("apikey")] public string? ApiKey { get; set; }

        [JsonPropertyName("name")] public string Name { get; set; }

        [JsonPropertyName("version")] public string Version { get; set; }
        [JsonPropertyName("variant")] public string Variant { get; set; }
        [JsonPropertyName("software")] public string Software { get; set; }

        [JsonPropertyName("printers")] public List<PrinterInfo> Printers { get; set; }

        [JsonPropertyName("servername")] public string ServerName { get; set; }

        [JsonPropertyName("serveruuid")] public string ServerUUID { get; set; }
    }
}
