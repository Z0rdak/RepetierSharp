using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Commands
{
    class StartJobCommand : ICommandData
    {
        [JsonPropertyName("id")]
        public int JobId { get; }
        [JsonIgnore]
        public string CommandIdentifier => CommandConstants.START_JOB;

        public StartJobCommand(int jobId)
        {
            JobId = jobId;
        }
    }
}
