using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Commands
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
        public string Action { get; set; }

        [JsonIgnore]
        public Type CommandType { get; set; }

        [JsonIgnore]
        public ICommandData Command { get; set; }

        [JsonPropertyName("data")]
        public Dictionary<string, object> Data { get; set; }

        [JsonPropertyName("printer")]
        public string Printer { get; set; }

        [JsonPropertyName("callback_id")]
        public int CallbackId { get; set; }

        public RepetierBaseCommand(ICommandData command, string printer, int callbackId, Type type)
        {
            this.Action = command.CommandIdentifier;
            this.Command = command;
            this.CallbackId = callbackId;
            this.CommandType = type;
            this.Data = JsonSerializer.Deserialize<Dictionary<string, object>>(JsonSerializer.Serialize(Command, type));
            this.Printer = printer;
        }

        public RepetierBaseCommand(Dictionary<string, object> data, string command, string printer, int callbackId)
        {
            this.Action = command;
            this.Command = null;
            this.CallbackId = callbackId;
            this.CommandType = null;
            this.Data = data;
            this.Printer = printer;
        }

        public override string ToString()
        {   
            return JsonSerializer.Serialize(this);
        }

        public byte[] ToBytes()
        {
            return Encoding.UTF8.GetBytes(ToString());
        }
    }
}