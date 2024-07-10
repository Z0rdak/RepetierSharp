using System;
using RepetierSharp.Models.Events;
using RepetierSharp.Models.Messages;

namespace RepetierSharp.Internal
{
    public sealed class RepetierClientEvents
    {
        public AsyncEvent<RepetierConnectedEventArgs> ConnectedEvent { get; } = new();
        public AsyncEvent<LoginRequiredEventArgs> LoginRequiredEvent = new();
        public AsyncEvent<LoginResultEventArgs> LoginResultEvent = new();
        public AsyncEvent<PermissionDeniedEventArgs> PermissionDeniedEvent = new();
        public AsyncEvent<SessionIdReceivedEventArgs> SessionIdReceivedEvent = new();
        public AsyncEvent<RepetierEventReceivedEventArgs> RepetierEventReceivedEvent = new();
        public AsyncEvent<RepetierResponseReceivedEventArgs> RepetierResponseReceivedEvent = new();
        public AsyncEvent<RawRepetierEventReceivedEventArgs> RawRepetierEventReceivedEvent = new();
        public AsyncEvent<RawRepetierResponseReceivedEventArgs> RawRepetierResponseReceivedEvent = new();
    }


    public sealed class SessionIdReceivedEventArgs : EventArgs
    {
        public SessionIdReceivedEventArgs(string sessionId)
        {
            SessionId = sessionId;
        }

        public string SessionId { get; }
    }


    public sealed class PermissionDeniedEventArgs : EventArgs
    {
        public PermissionDeniedEventArgs(int commandId)
        {
            CommandId = commandId;
        }

        public int CommandId { get; }
    }

    public sealed class RepetierEventReceivedEventArgs : EventArgs
    {
        public RepetierEventReceivedEventArgs(string eventName, string printer, IRepetierEvent? repetierEvent)
        {
            EventName = eventName;
            Printer = printer;
            RepetierEvent = repetierEvent;
        }

        public IRepetierEvent? RepetierEvent { get; }
        public string EventName { get; }
        public string Printer { get; }
    }

    public sealed class RawRepetierEventReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// </summary>
        /// <param name="eventName"> Name of the received event </param>
        /// <param name="printer"> Printer associated with the event or empty if global </param>
        /// <param name="eventPayload"> Event payload as byte array </param>
        public RawRepetierEventReceivedEventArgs(string eventName, string printer, byte[] eventPayload)
        {
            EventName = eventName;
            Printer = printer;
            EventPayload = eventPayload;
        }

        public byte[] EventPayload { get; }
        public string EventName { get; }
        public string Printer { get; }
    }

    public sealed class RepetierResponseReceivedEventArgs : EventArgs
    {
        public RepetierResponseReceivedEventArgs(int callbackId, string command, IRepetierMessage? message)
        {
            CallbackId = callbackId;
            Command = command;
            Message = message;
        }

        public string Command { get; }
        public int CallbackId { get; }
        public IRepetierMessage? Message { get; }
    }

    public sealed class RawRepetierResponseReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// </summary>
        /// <param name="callbackId"> CallBackId to identify the received command response </param>
        /// <param name="command"> Name of the command associated with the received response </param>
        /// <param name="responsePayload"> Command response payload </param>
        public RawRepetierResponseReceivedEventArgs(int callbackId, string command, byte[] responsePayload)
        {
            CallbackId = callbackId;
            Command = command;
            ResponsePayload = responsePayload;
        }

        public string Command { get; }
        public int CallbackId { get; }
        public byte[] ResponsePayload { get; }
    }


    public sealed class LoginResultEventArgs : EventArgs
    {
        public LoginResultEventArgs(LoginResponse loginResult)
        {
            LoginResult = loginResult;
        }

        public LoginResponse LoginResult { get; }
    }

    public sealed class LoginRequiredEventArgs : EventArgs
    {
    }


    public sealed class RepetierConnectedEventArgs : EventArgs
    {
        public RepetierConnectedEventArgs(string sessionId)
        {
            SessionId = sessionId;
        }

        public string SessionId { get; }
    }
}
