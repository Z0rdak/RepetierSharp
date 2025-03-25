using System.Text.Json.Serialization;
using RepetierSharp.Internal;
using RepetierSharp.Models.Communication;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Commands
{
    [CommandId(CommandConstants.START_JOB)]
    public class StartJobCommand(int jobId) : ICommandData
    {
        [JsonPropertyName("id")] public int JobId { get; } = jobId;

        [JsonIgnore] public string Action => CommandConstants.START_JOB;
    }
}
