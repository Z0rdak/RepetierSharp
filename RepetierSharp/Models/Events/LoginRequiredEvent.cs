using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Events
{
    public class LoginRequiredEvent : IRepetierEvent
    {
        [JsonPropertyName("session")]
        public string SessionId { get; set; }
        public LoginRequiredEvent() { }
    }

}