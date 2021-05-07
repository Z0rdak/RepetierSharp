using System;
using System.Collections.Generic;
using System.Text;

namespace RepetierMqtt.Models.Commands
{
    class ListJobsCommand : ICommandData
    {
        public string CommandIdentifier => "listJobs";

        private ListJobsCommand() { }

        public static ListJobsCommand Instance = new ListJobsCommand();
    }
}
