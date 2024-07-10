using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Requests
{
    public class ListJobsRequest : IRepetierRequest
    {
        public static ListJobsRequest Instance = new();

        private ListJobsRequest() { }

        [JsonIgnore] public string CommandIdentifier => CommandConstants.LIST_JOBS;
    }
}
