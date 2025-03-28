using System;
using RepetierSharp.Models.Events;
using RepetierSharp.Models.Communication;
using RepetierSharp.Models.Responses;

namespace RepetierSharp.Internal
{
    public sealed class RepetierServerEvents
    {
        public AsyncEvent<LogEntryEventArgs> LogEntryEvent { get; } = new();
        public AsyncEvent<PrinterListChangedEventArgs> PrinterListChangedEvent { get; } = new();
        public AsyncEvent<MessagesChangedEventArgs> MessagesChangedEvent { get; } = new();
        public AsyncEvent<ServerCommandEventArgs> CommandSendEvent { get; } = new();
        public AsyncEvent<CommandEventArgs> CommandFailedEvent { get; } = new();
        public AsyncEvent<ResponseEventArgs> ResponseReceivedEvent { get; } = new();
        
        public AsyncEvent<ServerEventEventArgs> EventReceivedEvent { get; } = new();
    }
    
    public sealed class ServerEventEventArgs(string eventName, IEventData? repetierEvent) : EventArgs
    {
        public IEventData? RepetierEvent { get; } = repetierEvent;
        public string EventName { get; } = eventName;
    }
    
    public sealed class ResponseEventArgs(RepetierResponse response) : EventArgs
    {
        public RepetierResponse Response { get; } = response;
    }
    
    public sealed class PrinterListChangedEventArgs(PrinterListChanged printerList) : EventArgs
    {

        public PrinterListChanged PrinterListChanged { get; } = printerList;
    }

    public sealed class MessagesChangedEventArgs(Message? message) : EventArgs
    {

        public Message? Message { get; } = message;
    }

    public sealed class CommandEventArgs(BaseCommand command) : EventArgs
    {
        public BaseCommand Command { get; } = command;
    }
    
    public sealed class ServerCommandEventArgs(ServerCommand command) : EventArgs
    {
        public ServerCommand Command { get; } = command;
    }

    public sealed class LogEntryEventArgs(LogEntry logEntry) : EventArgs
    {

        public LogEntry LogEntry { get; } = logEntry;
    }
}
