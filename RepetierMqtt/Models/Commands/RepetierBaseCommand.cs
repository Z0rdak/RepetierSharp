using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RepetierMqtt.Models.Commands
{
    /// <summary>
    /// Represents a WebSocket command for requesting data from the Repetier Server.
    /// A command is a JSON-object has the following structure (e.g):
    /// {"action":"ping","data":{},"printer":"MyPrinter","callback_id":545}
    /// source: https://www.repetier-server.com/manuals/programming/API/index.html
    /// </summary>
    public class RepetierBaseCommand
    {

        [JsonPropertyName("action")]
        public string Action { get; }

        [JsonPropertyName("data")]
        public ICommandData Command { get; }

        [JsonPropertyName("printer")]
        public string Printer { get; }

        [JsonPropertyName("callback_id")]
        public int CallbackId { get; }

        // TODO: GenerateId and store it into repetierconnection?
        // data are params in JSON format
        public RepetierBaseCommand(ICommandData command, string printer, int callbackId)
        {
            this.Action = command.CommandIdentifier;
            this.Command = command;
            this.Printer = printer;
            this.CallbackId = callbackId;
        }

        public override string ToString()
        {
            return JsonSerializer.Serialize(this);
        }

        public byte[] ToBytes()
        {
            return Encoding.UTF8.GetBytes(this.ToString());
        }
    }
}