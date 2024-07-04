using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Requests
{
    public class StopJobRequest : IRepetierRequest
    {
        [JsonIgnore]
        public string CommandIdentifier => CommandConstants.STOP_JOB;

        private StopJobRequest() { }

        public static StopJobRequest Instance => new StopJobRequest();
    }
}
