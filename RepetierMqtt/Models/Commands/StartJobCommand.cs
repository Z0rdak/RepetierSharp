using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace RepetierMqtt.Models.Commands
{
    class StartJobCommand : ICommandData
    {
        [JsonPropertyName("id")]
        public int JobId { get; }

        public string CommandIdentifier => "startJob";

        public StartJobCommand(int jobId)
        {
            JobId = jobId;
        }
    }
}
