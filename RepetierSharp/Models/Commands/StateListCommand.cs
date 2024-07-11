using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Commands
{
    public class StateListCommand : IRepetierCommand
    {
        public StateListCommand(bool includeHistory = false)
        {
            IncludeHistory = includeHistory;
        }

        [JsonPropertyName("includeHistory")] public bool IncludeHistory { get; }

        [JsonIgnore] public string CommandIdentifier => CommandConstants.STATE_LIST;
    }
}
