using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Commands
{
    class ListJobsCommand : ICommandData
    {
        [JsonIgnore]
        public string CommandIdentifier => CommandConstants.LIST_JOBS;

        private ListJobsCommand() { }

        public static ListJobsCommand Instance = new ListJobsCommand();
    }
}
