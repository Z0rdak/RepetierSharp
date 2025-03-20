using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Commands
{
    public class ListJobsCommand : IRepetierCommand
    {
        public static ListJobsCommand AllJobs => new(true);
        public static ListJobsCommand QueuedJobs => new(false);

        public ListJobsCommand(bool includeRunning = false)
        {
            this.IncludeRunning = includeRunning;
        }
        [JsonPropertyName("includeRunning")]
        public bool IncludeRunning { get; set; }

        [JsonIgnore] public string CommandIdentifier => CommandConstants.LIST_JOBS;
    }
}
