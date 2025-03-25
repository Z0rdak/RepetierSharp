using System.Text.Json.Serialization;
using RepetierSharp.Internal;
using RepetierSharp.Models.Communication;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Commands
{
    [CommandId(CommandConstants.UPDATE_AVAILABLE)]
    public class UpdateAvailableCommand : ICommandData
    {
        private UpdateAvailableCommand() { }

        public static UpdateAvailableCommand Instance => new();

        [JsonIgnore] public string Action => CommandConstants.UPDATE_AVAILABLE;
    }
}
