using System.Text.Json.Serialization;
using RepetierSharp.Models.Communication;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Events
{
    [EventId(EventConstants.LOGIN_REQUIRED)]
    public class LoginRequired : IEventData
    {
        [JsonPropertyName("session")] public string SessionId { get; set; }
    }
}
