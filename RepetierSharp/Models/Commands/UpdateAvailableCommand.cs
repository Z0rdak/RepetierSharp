using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Commands
{
    public class UpdateAvailableCommand : IRepetierCommand
    {
        private UpdateAvailableCommand() { }

        public static UpdateAvailableCommand Instance => new();

        [JsonIgnore] public string CommandIdentifier => CommandConstants.UPDATE_AVAILABLE;
    }
}
