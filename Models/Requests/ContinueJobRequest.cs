using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Requests
{
    public class ContinueJobRequest : IRepetierRequest
    {
        [JsonIgnore]
        public string CommandIdentifier => CommandConstants.CONTINUE_JOB;

        private ContinueJobRequest() { }

        public static ContinueJobRequest Instance => new ContinueJobRequest();
    }
}
