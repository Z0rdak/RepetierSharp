using System.Text.Json.Serialization;
using RepetierSharp.Internal;
using RepetierSharp.Models.Communication;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Commands
{
    [CommandId(CommandConstants.LIST_MODELS)]
    public class ListModelsCommand : ICommandData
    {
        public static ListModelsCommand Instance => new ListModelsCommand();
        private ListModelsCommand() { }
        [JsonIgnore] public string Action => CommandConstants.LIST_MODELS;
    }
}
