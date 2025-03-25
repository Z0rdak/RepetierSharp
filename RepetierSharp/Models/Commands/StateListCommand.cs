using System.Text.Json.Serialization;
using RepetierSharp.Internal;
using RepetierSharp.Models.Communication;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Commands
{
    [CommandId(CommandConstants.STATE_LIST)]
    public class StateListCommand(bool includeHistory = false) : ICommandData
    {
        [JsonPropertyName("includeHistory")] public bool IncludeHistory { get; } = includeHistory;

        [JsonIgnore] public string Action => CommandConstants.STATE_LIST;
    }
}
