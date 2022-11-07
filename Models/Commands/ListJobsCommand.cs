using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Commands
{
    public class ListJobsCommand : ICommandData
    {
        [JsonIgnore]
        public string CommandIdentifier => CommandConstants.LIST_JOBS;

        private ListJobsCommand() { }

        public static ListJobsCommand Instance = new ListJobsCommand();
    }
}
