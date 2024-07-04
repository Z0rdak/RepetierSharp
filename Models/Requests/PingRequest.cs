using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Requests
{
    public class PingRequest : IRepetierRequest
    {
        [JsonIgnore]
        public string CommandIdentifier => CommandConstants.PING;

        private PingRequest() { }

        public static PingRequest Instance => new PingRequest();
    }
}
