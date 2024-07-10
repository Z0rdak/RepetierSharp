using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Requests
{
    public class StopJobRequest : IRepetierRequest
    {
        private StopJobRequest() { }

        public static StopJobRequest Instance => new();

        [JsonIgnore] public string CommandIdentifier => CommandConstants.STOP_JOB;
    }
}
