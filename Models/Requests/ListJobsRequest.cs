using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Requests
{
    public class ListJobsRequest : IRepetierRequest
    {
        [JsonIgnore]
        public string CommandIdentifier => CommandConstants.LIST_JOBS;

        private ListJobsRequest() { }

        public static ListJobsRequest Instance = new ListJobsRequest();
    }
}
