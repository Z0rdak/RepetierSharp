using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Commands
{
    public class UpdateAvailableCommand : ICommandData
    {
        private UpdateAvailableCommand() { }

        public static UpdateAvailableCommand Instance => new();

        [JsonIgnore] public string Action => CommandConstants.UPDATE_AVAILABLE;
    }
}
