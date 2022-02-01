using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Events
{
    // TODO: SendMoves & hiveMoves commands
    public class MoveEvent : IRepetierEvent
    {
        [JsonPropertyName("x")]
        public float X { get; set; }

        [JsonPropertyName("y")]
        public float Y { get; set; }

        [JsonPropertyName("z")]
        public float Z { get; set; }

        [JsonPropertyName("e")]
        public float Exruder { get; set; }

        /// <summary>
        /// mm/s
        /// </summary>
        [JsonPropertyName("speed")]
        public float Speed { get; set; }

        /// <summary>
        /// relative (bool = true) = positions are relative
        /// </summary>
        [JsonPropertyName("relative")]
        public bool RelativeMove { get; set; }

        public MoveEvent() { }
    }
}
