using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace RepetierMqtt.Models.Commands
{
    class RemoveJobCommand : ICommandData
    {
        [JsonPropertyName("id")]
        public int JobId { get; }

        public string CommandIdentifier => "removeJob";

        public RemoveJobCommand(int jobId)
        {
            JobId = jobId;
        }
    }
}
