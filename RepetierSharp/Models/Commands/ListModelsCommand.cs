using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Commands
{
    public class ListModelsCommand : ICommandData
    {
        public static ListModelsCommand Instance => new ListModelsCommand();
        private ListModelsCommand() { }
        [JsonIgnore] public string Action => CommandConstants.LIST_MODELS;
    }
}
