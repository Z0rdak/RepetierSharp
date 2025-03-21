using System;
using RepetierSharp.Models;
using RepetierSharp.Models.Events;
using RepetierSharp.Models.Messages;
using RestSharp;
using Websocket.Client;

namespace RepetierSharp.Internal
{
    public sealed class RepetierClientEvents
    {
        public AsyncEvent<ConnectedEventArgs> ConnectedEvent { get; } = new();
        public AsyncEvent<DisconnectedEventArgs> DisconnectedEvent { get; } = new();
        public AsyncEvent<LoginRequiredEventArgs> LoginRequiredEvent { get; } = new();
        public AsyncEvent<LoginResultEventArgs> LoginResultEvent { get; } = new();
        public AsyncEvent<PermissionDeniedEventArgs> PermissionDeniedEvent { get; } = new();
        public AsyncEvent<SessionIdReceivedEventArgs> SessionIdReceivedEvent { get; } = new();
        public AsyncEvent<UserCredentialsReceivedEventArgs> CredentialsReceivedEvent { get; } = new();
        public AsyncEvent<EventReceivedEventArgs> EventReceivedEvent { get; } = new();
        public AsyncEvent<ResponseReceivedEventArgs> ResponseReceivedEvent { get; } = new();
        public AsyncEvent<RawRepetierEventReceivedEventArgs> RawEventReceivedEvent { get; } = new();
        public AsyncEvent<RawResponseReceivedEventArgs> RawResponseReceivedEvent { get; } = new();
        public AsyncEvent<CommandEventArgs> CommandSendEvent { get; } = new();
        public AsyncEvent<HttpContextEventArgs> HttpRequestFailedEvent { get; } = new();
        public AsyncEvent<CommandEventArgs> CommandFailedEvent { get; } = new();
    }
    
    public sealed class HttpContextEventArgs : EventArgs
    {
        public HttpContextEventArgs(RestRequest request, RestResponse response)
        {
            Request = request;
            Response = response;
        }
        public RestRequest Request { get; }
        public RestResponse Response { get; }
    }

    public sealed class UserCredentialsReceivedEventArgs : EventArgs
    {
        public UserCredentialsReceivedEventArgs(UserCredentials userCredentials)
        {
            UserCredentials = userCredentials;
        }

        public UserCredentials UserCredentials { get; }
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

    public sealed class EventReceivedEventArgs : EventArgs
    {
        public EventReceivedEventArgs(string eventName, string printer, IRepetierEvent? repetierEvent)
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

    public sealed class ResponseReceivedEventArgs : EventArgs
    {
        public ResponseReceivedEventArgs(int callbackId, string command, IRepetierMessage? message)
        {
            CallbackId = callbackId;
            Command = command;
            Message = message;
        }

        public string Command { get; }
        public int CallbackId { get; }
        public IRepetierMessage? Message { get; }
    }

    public sealed class RawResponseReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// </summary>
        /// <param name="callbackId"> CallBackId to identify the received command response </param>
        /// <param name="command"> Name of the command associated with the received response </param>
        /// <param name="responsePayload"> Command response payload </param>
        public RawResponseReceivedEventArgs(int callbackId, string command, byte[] responsePayload)
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

    public sealed class DisconnectedEventArgs : EventArgs
    {
        public DisconnectedEventArgs(DisconnectionInfo info)
        {
            Info = info;
        }
        public DisconnectionInfo Info { get; }
    }

    public sealed class ConnectedEventArgs : EventArgs
    {
        public ConnectedEventArgs(bool reconnect = false)
        {
            Reconnect = reconnect;
        }
        public bool Reconnect { get; }
        
    }
}
