using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Commands
{
    public class StartJobCommand : IRepetierCommand
    {
        public StartJobCommand(int jobId)
        {
            JobId = jobId;
        }

        [JsonPropertyName("id")] public int JobId { get; }

        [JsonIgnore] public string CommandIdentifier => CommandConstants.START_JOB;
    }
}
