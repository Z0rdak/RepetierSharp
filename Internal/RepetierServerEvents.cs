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
        public AsyncEvent<MessagesReceivedEventArgs> MessagesReceivedEvent { get; } = new();
        public AsyncEvent<PrinterStatesReceivedEventArgs> PrinterStatesReceivedEvent { get; } = new();
        public AsyncEvent<RepetierRequestEventArgs> RepetierRequestSendEvent { get; } = new();
        public AsyncEvent<RepetierRequestFailedEventArgs> RepetierRequestFailedEvent { get; } = new();
    }
    
    public sealed class PrinterListChangedEventArgs : EventArgs
    {
        public PrinterListChangedEventArgs(ListPrinterResponse printerList)
        {
            ListPrinterResponse = printerList;
        }

        public ListPrinterResponse ListPrinterResponse { get; }
    }

    public sealed class MessagesReceivedEventArgs : EventArgs
    {
        public MessagesReceivedEventArgs(MessageList messages)
        {
            MessageList = messages;
        }

        public MessageList MessageList { get; }
    }

    public sealed class PrinterStatesReceivedEventArgs : EventArgs
    {
        public PrinterStatesReceivedEventArgs(StateListResponse printerStates)
        {
            StateListResponse = printerStates;
        }

        public StateListResponse StateListResponse { get; }
    }

    public sealed class RepetierRequestEventArgs : EventArgs
    {
        public RepetierRequestEventArgs(RepetierBaseRequest request)
        {
            RepetierBaseRequest = request;
        }

        public RepetierBaseRequest RepetierBaseRequest { get; }
    }

    public sealed class RepetierRequestFailedEventArgs : EventArgs
    {
        public RepetierRequestFailedEventArgs(RepetierBaseRequest request)
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
