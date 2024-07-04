using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Requests
{
    public class StateListRequest : IRepetierRequest
    {
        [JsonPropertyName("includeHistory")]
        public bool IncludeHistory { get; }
        [JsonIgnore]
        public string CommandIdentifier => CommandConstants.STATE_LIST;

        public StateListRequest(bool includeHistory = false)
        {
            IncludeHistory = includeHistory;
        }
    }
}
