using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Requests
{
    public class PingRequest : IRepetierRequest
    {
        private PingRequest() { }

        public static PingRequest Instance => new();

        [JsonIgnore] public string CommandIdentifier => CommandConstants.PING;
    }
}
