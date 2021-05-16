using System;
using System.Collections.Generic;
using System.Text;

namespace RepetierMqtt.Models.Commands
{
    public class ContinueJobCommand : ICommandData
    {
        public string CommandIdentifier => CommandConstants.CONTINUE_JOB;

        private ContinueJobCommand() { }

        public static ContinueJobCommand Instance => new ContinueJobCommand();
    }
}
