using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Requests
{
    public class LogoutRequest : IRepetierRequest
    {
        private LogoutRequest() { }

        public static LogoutRequest Instance => new();

        [JsonIgnore] public string CommandIdentifier => CommandConstants.LOGOUT;
    }
}
