using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Requests
{
    public class UpdateAvailableRequest : IRepetierRequest
    {
        private UpdateAvailableRequest() { }

        public static UpdateAvailableRequest Instance => new();

        [JsonIgnore] public string CommandIdentifier => CommandConstants.UPDATE_AVAILABLE;
    }
}
