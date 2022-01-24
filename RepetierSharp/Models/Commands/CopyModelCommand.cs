using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Commands
{
    public class CopyModelCommand : ICommandData
    {
        public string CommandIdentifier => CommandConstants.COPY_MODEL;

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
