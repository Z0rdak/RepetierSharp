﻿using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Requests
{
    public class DeactivateRequest : IRepetierRequest
    {
        [JsonPropertyName("printer")]
        public string PrinterSlug { get; }
        [JsonIgnore]
        public string CommandIdentifier => CommandConstants.DEACTIVATE;

        public DeactivateRequest(string printer)
        {
            PrinterSlug = printer;
        }
    }
}