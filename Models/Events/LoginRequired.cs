using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Events
{
    [EventId("loginRequired")]
    public class LoginRequired : IRepetierEvent
    {
        [JsonPropertyName("session")]
        public string SessionId { get; set; }
        public LoginRequired() { }
    }

}
