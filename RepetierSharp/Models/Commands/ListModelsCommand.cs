using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Commands
{
    public class ListModelsCommand : IRepetierCommand
    {
        public static ListModelsCommand Instance => new ListModelsCommand();
        private ListModelsCommand() { }
        [JsonIgnore] public string CommandIdentifier => CommandConstants.LIST_MODELS;
    }
}
