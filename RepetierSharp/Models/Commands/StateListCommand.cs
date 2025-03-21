using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Commands
{
    public class StateListCommand : ICommandData
    {
        public StateListCommand(bool includeHistory = false)
        {
            IncludeHistory = includeHistory;
        }

        [JsonPropertyName("includeHistory")] public bool IncludeHistory { get; }

        [JsonIgnore] public string Action => CommandConstants.STATE_LIST;
    }
}
