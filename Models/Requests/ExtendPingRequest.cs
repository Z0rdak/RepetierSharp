using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Requests
{
    public class ExtendPingRequest : IRepetierRequest
    {
        [JsonIgnore]
        public string CommandIdentifier => CommandConstants.PING;

        public uint Timeout { get; set; }

        public ExtendPingRequest(uint timeout)
        {
            Timeout = timeout;
        }

    }
}
