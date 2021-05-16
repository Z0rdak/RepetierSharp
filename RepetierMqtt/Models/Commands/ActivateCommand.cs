using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;

namespace RepetierMqtt.Models.Commands
{
    public class ActivateCommand : ICommandData
    {
        [JsonPropertyName("printer")]
        public string PrinterSlug { get; }

        public string CommandIdentifier => CommandConstants.ACTIVATE;

        public ActivateCommand(string printer)
        {
            PrinterSlug = printer;
        }
    }
}
