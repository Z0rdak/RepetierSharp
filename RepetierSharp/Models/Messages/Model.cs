using RepetierSharp.Models.Messages;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace RepetierSharp.Models
{
    /// <summary>
    /// Represents a g-code file stored in the printer. It also contains some statistical data.
    /// </summary>
    public class Model : IRepetierMessage
    {
        [JsonPropertyName("analysed")]
        public int Analysed { get; set; }

        [JsonPropertyName("created")]
        public long Created { get; set; }

        [JsonPropertyName("extruderUsage")]
        public List<double> ExtruderUsage { get; set; }

        [JsonPropertyName("filamentTotal")]
        public double FilamentTotal { get; set; }

        [JsonPropertyName("id")]
        public int Id { get; set; }

        [JsonPropertyName("layer")]
        public int Layer { get; set; }

        [JsonPropertyName("length")]
        public int Length { get; set; }

        [JsonPropertyName("lines")]
        public int Lines { get; set; }

        [JsonPropertyName("name")]
        public string Name { get; set; }

        [JsonPropertyName("printTime")]
        public double PrintTime { get; set; }

        [JsonPropertyName("printed")]
        public int Printed { get; set; }

        [JsonPropertyName("printedTimeComp")]
        public int PrintedTimeComp { get; set; }

        [JsonPropertyName("state")]
        public string State { get; set; }

        [JsonPropertyName("volumeTotal")]
        public int VolumeTotal { get; set; }

        [JsonPropertyName("volumeUsage")]
        public List<int> VolumeUsage { get; set; }

        public Model() { }
    }

}
