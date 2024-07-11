using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Requests
{
    public class ExtendPingRequest : IRepetierRequest
    {
        public ExtendPingRequest(uint timeout)
        {
            Timeout = timeout;
        }

        public uint Timeout { get; set; }

        [JsonIgnore] public string CommandIdentifier => CommandConstants.PING;
    }
}
