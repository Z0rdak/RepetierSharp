using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Commands
{
    public class ListJobsCommand : IRepetierCommand
    {
        public static ListJobsCommand Instance = new();

        private ListJobsCommand() { }

        [JsonIgnore] public string CommandIdentifier => CommandConstants.LIST_JOBS;
    }
}
