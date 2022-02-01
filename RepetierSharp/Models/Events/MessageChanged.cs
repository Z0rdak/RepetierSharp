using RepetierSharp.Models.Messages;
using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Events
{
    [EventId("messagesChanged")]
    public class MessagesChanged : Message, IRepetierEvent
    {
        /* Gets triggered when a new message gets available. That way you do not need to poll for new messages, 
         * only once at the connection start. In case of a new message it is also transmitted as payload. When a message gets removed, the payload is empty. */
    }
}