using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Commands
{
    public class UpdateAvailableCommand : ICommandData
    {
        [JsonIgnore]
        public string CommandIdentifier => CommandConstants.UPDATE_AVAILABLE;

        public static UpdateAvailableCommand Instance => new UpdateAvailableCommand();

        private UpdateAvailableCommand() { }
    }
}
