using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace RepetierMqtt.Models.Commands
{
    public class DeactivateCommand : ICommandData
    {
        [JsonPropertyName("printer")]
        public string PrinterSlug { get; }

        public string CommandIdentifier => CommandConstants.DEACTIVATE;

        public DeactivateCommand(string printer)
        {
            PrinterSlug = printer;
        }
    }
}
