using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Commands
{
    public class JobInfoCommand : ICommandData
    {
        [JsonPropertyName("id")]
        public int JobId { get; }
        [JsonIgnore]
        public string CommandIdentifier => CommandConstants.JOB_INFO;

        public JobInfoCommand(int jobId)
        {
            JobId = jobId;
        }
    }
}
