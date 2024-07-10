using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Requests
{
    public class StateListRequest : IRepetierRequest
    {
        public StateListRequest(bool includeHistory = false)
        {
            IncludeHistory = includeHistory;
        }

        [JsonPropertyName("includeHistory")] public bool IncludeHistory { get; }

        [JsonIgnore] public string CommandIdentifier => CommandConstants.STATE_LIST;
    }
}
