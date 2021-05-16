using System;
using System.Collections.Generic;
using System.Text;

namespace RepetierMqtt.Models.Commands
{
    public class StopJobCommand : ICommandData
    {
        public string CommandIdentifier => CommandConstants.STOP_JOB;

        private StopJobCommand() { }

        public static StopJobCommand Instance => new StopJobCommand();
    }
}
