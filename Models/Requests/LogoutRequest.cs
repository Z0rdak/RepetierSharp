using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Requests
{
    public class LogoutRequest : IRepetierRequest
    {
        [JsonIgnore]
        public string CommandIdentifier => CommandConstants.LOGOUT;

        private LogoutRequest() { }

        public static LogoutRequest Instance => new LogoutRequest();
    }
}
