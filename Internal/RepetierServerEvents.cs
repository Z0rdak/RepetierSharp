using System;
using RepetierSharp.Models.Events;
using RepetierSharp.Models.Messages;
using RepetierSharp.Models.Requests;

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
        public PrinterListChangedEventArgs(ListPrinterResponse printerList)
        {
            ListPrinterResponse = printerList;
        }

        public ListPrinterResponse ListPrinterResponse { get; }
    }

    public sealed class MessagesChangedEventArgs : EventArgs
    {
        public MessagesChangedEventArgs(Message? message)
        {
            Message = message;
        }

        public Message? Message { get; }
    }

    public sealed class RepetierRequestEventArgs : EventArgs
    {
        public RepetierRequestEventArgs(RepetierBaseRequest request)
        {
            RepetierBaseRequest = request;
        }

        public RepetierBaseRequest RepetierBaseRequest { get; }
    }

    public sealed class LogEntryEventArgs : EventArgs
    {
        public LogEntryEventArgs(Log logEntry)
        {
            LogEntry = logEntry;
        }

        public Log LogEntry { get; }
    }
}
