using System;
using System.Collections.Generic;
using System.Text;

namespace RepetierMqtt.Models.Commands
{
    class ListJobsCommand : ICommandData
    {
        public string CommandIdentifier => CommandConstants.LIST_JOBS;

        private ListJobsCommand() { }

        public static ListJobsCommand Instance = new ListJobsCommand();
    }
}
