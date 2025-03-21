using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Commands
{
    public class ListJobsCommand : ICommandData
    {
        public static ListJobsCommand AllJobs => new(true);
        public static ListJobsCommand QueuedJobs => new(false);

        public ListJobsCommand(bool includeRunning = false)
        {
            this.IncludeRunning = includeRunning;
        }
        [JsonPropertyName("includeRunning")]
        public bool IncludeRunning { get; set; }

        [JsonIgnore] public string Action => CommandConstants.LIST_JOBS;
    }
}
