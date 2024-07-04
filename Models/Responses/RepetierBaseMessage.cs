using System.Text.Json.Serialization;
using RepetierSharp.Models.Events;

namespace RepetierSharp.Models.Messages
{
    /* Message sent by the server as a response 
     * to a command or containing a event message */
    public class RepetierBaseMessage
    {
        /// <summary>
        /// A callback_id of -1 indicates that the message contains events and is not a response to a command.
        /// </summary>
        [JsonPropertyName("callback_id")]
        public int CallBackId { get; set; }

        [JsonPropertyName("session")]
        public string SessionId { get; set; }

        [JsonPropertyName("data")]
        public IRepetierMessage Data { get; set; }

        /// <summary>
        /// Indicates if the message contains events
        /// </summary>
        [JsonPropertyName("eventList")]
        public bool? HasEvents { get; set; }

        public RepetierBaseMessage() { }

    }
}
