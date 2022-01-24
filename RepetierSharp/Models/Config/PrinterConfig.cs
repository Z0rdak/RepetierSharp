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
        public Connection Connection { get; }

        [JsonPropertyName("extruders")]
        public List<Extruder> Extruders { get; }

        [JsonPropertyName("general")]
        public General General { get; }

        [JsonPropertyName("heatedBed")]
        public HeatedBed HeatedBed { get; }

        [JsonPropertyName("movement")]
        public Movement Movement { get; }

        [JsonPropertyName("quickCommands")]
        public List<QuickCommand> QuickCommands { get; }

        [JsonPropertyName("shape")]
        public Shape Shape { get; }

        [JsonPropertyName("webcam")]
        public Webcam Webcam { get; }
    }

}
