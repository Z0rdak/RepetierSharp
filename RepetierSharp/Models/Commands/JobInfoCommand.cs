using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Commands
{
    public class JobInfoCommand : IRepetierCommand
    {
        public JobInfoCommand(int jobId)
        {
            JobId = jobId;
        }

        [JsonPropertyName("id")] public int JobId { get; }

        [JsonIgnore] public string CommandIdentifier => CommandConstants.JOB_INFO;
    }
}
