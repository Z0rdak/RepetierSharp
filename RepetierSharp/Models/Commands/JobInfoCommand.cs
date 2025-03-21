using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Commands
{
    public class JobInfoCommand : ICommandData
    {
        public JobInfoCommand(int jobId)
        {
            JobId = jobId;
        }

        [JsonPropertyName("id")] public int JobId { get; }

        [JsonIgnore] public string Action => CommandConstants.JOB_INFO;
    }
}
