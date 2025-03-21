using System;
using RepetierSharp.Models.Events;
using RepetierSharp.Models.Messages;
using RepetierSharp.Models.Commands;

namespace RepetierSharp.Internal
{
    public sealed class RepetierServerEvents
    {
        public AsyncEvent<LogEntryEventArgs> LogEntryEvent { get; } = new();
        public AsyncEvent<PrinterListChangedEventArgs> PrinterListChangedEvent { get; } = new();
        public AsyncEvent<MessagesChangedEventArgs> MessagesChangedEvent { get; } = new();
    }
    
    public sealed class PrinterListChangedEventArgs : EventArgs
    {
        public PrinterListChangedEventArgs(PrinterListChanged printerList)
        {
            PrinterListChanged = printerList;
        }

        public PrinterListChanged PrinterListChanged { get; }
    }

    public sealed class MessagesChangedEventArgs : EventArgs
    {
        public MessagesChangedEventArgs(Message? message)
        {
            Message = message;
        }

        public Message? Message { get; }
    }

    public sealed class CommandEventArgs : EventArgs
    {
        public CommandEventArgs(BaseCommand command)
        {
            Command = command;
        }

        public BaseCommand Command { get; }
    }

    public sealed class LogEntryEventArgs : EventArgs
    {
        public LogEntryEventArgs(LogEntry logEntry)
        {
            LogEntry = logEntry;
        }

        public LogEntry LogEntry { get; }
    }
}
