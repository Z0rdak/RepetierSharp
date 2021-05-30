using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace RepetierMqtt.Models.Events
{
    // TODO: SendMoves & hiveMoves commands
    public class MoveEvent : IRepetierEvent
    {
        [JsonPropertyName("x")]
        public float X { get; }

        [JsonPropertyName("y")]
        public float Y { get; }

        [JsonPropertyName("z")]
        public float Z { get; }

        [JsonPropertyName("e")]
        public float Exruder { get; }

        /// <summary>
        /// mm/s
        /// </summary>
        [JsonPropertyName("speed")]
        public float Speed { get; }

        /// <summary>
        /// relative (bool = true) = positions are relative
        /// </summary>
        [JsonPropertyName("relative")]
        public bool RelativeMove { get; }

        public MoveEvent() { }
    }
}
