using System.Text.Json.Serialization;
using RepetierSharp.Util;

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
