using System;
using System.Collections.Generic;
using System.Text;

namespace RepetierSharp.Models.Commands
{
    public class UpdateAvailableCommand : ICommandData
    {
        public string CommandIdentifier => CommandConstants.UPDATE_AVAILABLE;

        public static UpdateAvailableCommand Instance => new UpdateAvailableCommand();

        private UpdateAvailableCommand() { }
    }
}
