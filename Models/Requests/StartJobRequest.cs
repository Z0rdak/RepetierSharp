using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Requests
{
    public class StartJobRequest : IRepetierRequest
    {
        [JsonPropertyName("id")]
        public int JobId { get; }
        [JsonIgnore]
        public string CommandIdentifier => CommandConstants.START_JOB;

        public StartJobRequest(int jobId)
        {
            JobId = jobId;
        }
    }
}
