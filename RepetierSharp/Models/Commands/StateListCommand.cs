using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Commands
{
    public class StateListCommand : ICommandData
    {
        [JsonPropertyName("includeHistory")]
        public bool IncludeHistory { get; }

        public string CommandIdentifier => CommandConstants.STATE_LIST;

        public StateListCommand(bool includeHistory = false)
        {
            IncludeHistory = includeHistory;
        }
    }
}
