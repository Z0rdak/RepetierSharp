using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Commands
{
    public class RemoveJobCommand : ICommandData
    {
        [JsonPropertyName("id")]
        public int JobId { get; }
        [JsonIgnore]
        public string CommandIdentifier => CommandConstants.REMOVE_JOB;

        public RemoveJobCommand(int jobId)
        {
            JobId = jobId;
        }
    }
}
