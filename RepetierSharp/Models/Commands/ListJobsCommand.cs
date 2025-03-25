using System.Text.Json.Serialization;
using RepetierSharp.Internal;
using RepetierSharp.Models.Communication;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Commands
{
    [CommandId(CommandConstants.LIST_JOBS)]
    public class ListJobsCommand(bool includeRunning = false) : ICommandData
    {
        public static ListJobsCommand AllJobs => new(true);
        public static ListJobsCommand QueuedJobs => new(false);

        [JsonPropertyName("includeRunning")]
        public bool IncludeRunning { get; set; } = includeRunning;

        [JsonIgnore] public string Action => CommandConstants.LIST_JOBS;
    }
}
