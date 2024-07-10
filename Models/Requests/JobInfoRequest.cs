﻿using System.Text.Json.Serialization;
using RepetierSharp.Util;

namespace RepetierSharp.Models.Requests
{
    public class JobInfoRequest : IRepetierRequest
    {
        public JobInfoRequest(int jobId)
        {
            JobId = jobId;
        }

        [JsonPropertyName("id")] public int JobId { get; }

        [JsonIgnore] public string CommandIdentifier => CommandConstants.JOB_INFO;
    }
}
