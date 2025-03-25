using System.Text.Json.Serialization;
using RepetierSharp.Internal;
using RepetierSharp.Models.Communication;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Commands
{
    [CommandId(CommandConstants.REMOVE_JOB)]
    public class RemoveJobCommand(int jobId) : ICommandData
    {
        [JsonPropertyName("id")] public int JobId { get; } = jobId;

        [JsonIgnore] public string Action => CommandConstants.REMOVE_JOB;
    }
}
