using System.Text.Json.Serialization;
using RepetierSharp.Models.Commands;

namespace RepetierSharp.Models.Communication
{
    public class BaseCommand(string action, ICommandData data, string printer, int callbackId)
    {
        [JsonPropertyName("action")] public string Action { get; set; } = action;
        [JsonPropertyName("data")] public ICommandData Data { get; set; } = data;
        [JsonPropertyName("printer")] public string Printer { get; set; } = printer;
        [JsonPropertyName("callback_id")] public int CallbackId { get; set; } = callbackId;
    }

    public class PrinterCommand(string action, ICommandData data, string printer, int callbackId) : BaseCommand(action, data, printer, callbackId);

    public class ServerCommand(string action, ICommandData data, int callbackId) : BaseCommand(action, data, "", callbackId);
}
