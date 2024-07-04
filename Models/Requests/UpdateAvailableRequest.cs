using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Requests
{
    public class UpdateAvailableRequest : IRepetierRequest
    {
        [JsonIgnore]
        public string CommandIdentifier => CommandConstants.UPDATE_AVAILABLE;

        public static UpdateAvailableRequest Instance => new UpdateAvailableRequest();

        private UpdateAvailableRequest() { }
    }
}
