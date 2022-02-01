using RepetierSharp.Models.Events;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace RepetierSharp.Config
{
    public class PrinterConfig : IRepetierEvent
    {
        [JsonPropertyName("connection")]
        public Connection Connection { get; set; }

        [JsonPropertyName("extruders")]
        public List<Extruder> Extruders { get; set; }

        [JsonPropertyName("general")]
        public General General { get; set; }

        [JsonPropertyName("heatedBed")]
        public HeatedBed HeatedBed { get; set; }

        [JsonPropertyName("movement")]
        public Movement Movement { get; set; }

        [JsonPropertyName("quickCommands")]
        public List<QuickCommand> QuickCommands { get; set; }

        [JsonPropertyName("shape")]
        public Shape Shape { get; set; }

        [JsonPropertyName("webcam")]
        public Webcam Webcam { get; set; }
    }

}
