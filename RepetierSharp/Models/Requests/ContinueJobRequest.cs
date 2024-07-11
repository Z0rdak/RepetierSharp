using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Requests
{
    public class ContinueJobRequest : IRepetierRequest
    {
        private ContinueJobRequest() { }

        public static ContinueJobRequest Instance => new();

        [JsonIgnore] public string CommandIdentifier => CommandConstants.CONTINUE_JOB;
    }
}
