using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace RepetierMqtt.Models.Commands
{
    public class CopyModelCommand : ICommandData
    {
        public string CommandIdentifier => "copyModel";

        [JsonPropertyName("id")]
        public int Id { get; }

        [JsonPropertyName("autostart")]
        public bool Autostart { get; }

        public CopyModelCommand(int modelId, bool autostart = true) 
        {
            this.Id = modelId;
            this.Autostart = autostart;
        }
    }
}
