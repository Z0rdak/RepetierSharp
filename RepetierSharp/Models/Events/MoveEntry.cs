using System.Text.Json.Serialization;
using RepetierSharp.Models.Communication;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Events
{
    [EventId(EventConstants.MOVE)]
    public class MoveEntry : IEventData
    {
        [JsonPropertyName("x")] public float X { get; set; }

        [JsonPropertyName("y")] public float Y { get; set; }

        [JsonPropertyName("z")] public float Z { get; set; }

        [JsonPropertyName("e")] public float Extruder { get; set; }

        /// <summary>
        ///     mm/s
        /// </summary>
        [JsonPropertyName("speed")]
        public float Speed { get; set; }

        /// <summary>
        ///     relative (bool = true) = positions are relative
        /// </summary>
        [JsonPropertyName("relative")]
        public bool RelativeMove { get; set; }
    }
}
