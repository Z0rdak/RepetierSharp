using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Requests
{
    public class RemoveJobRequest : IRepetierRequest
    {
        [JsonPropertyName("id")]
        public int JobId { get; }
        [JsonIgnore]
        public string CommandIdentifier => CommandConstants.REMOVE_JOB;

        public RemoveJobRequest(int jobId)
        {
            JobId = jobId;
        }
    }
}
