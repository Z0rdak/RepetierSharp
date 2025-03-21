using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Commands
{
    public class RemoveJobCommand : ICommandData
    {
        public RemoveJobCommand(int jobId)
        {
            JobId = jobId;
        }

        [JsonPropertyName("id")] public int JobId { get; }

        [JsonIgnore] public string Action => CommandConstants.REMOVE_JOB;
    }
}
