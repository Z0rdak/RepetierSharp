using System.Text.Json.Serialization;

namespace RepetierSharp.Models.Commands
{
    public interface IRepetierCommand
    {
        [JsonIgnore] public string CommandIdentifier { get; }
    }

    public interface ICommand { }
    
    public abstract class BaseCommand : ICommand
    {
        [JsonPropertyName("action")] public string Action { get; set; }
        
        [JsonPropertyName("data")] ICommandData Data { get; set; }

        [JsonPropertyName("callback_id")] public int CallbackId { get; set; }
    }
    
    public abstract class PrinterCommand : BaseCommand
    {
        [JsonPropertyName("printer")] public string Printer { get; set; }
    }

    public abstract class ServerCommand : BaseCommand
    {
        [JsonPropertyName("printer")] public string Printer { get; } = "";
    }
}
