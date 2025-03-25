using RepetierSharp.Models.Communication;
using RepetierSharp.Models.Responses;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Events
{
    /// <summary>
    /// Gets triggered when a new message gets available.
    /// <br></br>
    /// That way you do not need to poll for new messages, only once at the connection start. <br></br>
    /// In case of a new message it is also transmitted as payload.
    /// <br></br>
    /// When a message gets removed, the payload is empty.
    /// </summary>
    [EventId(EventConstants.MESSAGES_CHANGED)]
    public class MessagesChanged : Message, IEventData { }
}
