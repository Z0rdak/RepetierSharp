using System;
using RepetierSharp.Models;
using RepetierSharp.Models.Communication;
using RepetierSharp.Models.Events;
using RepetierSharp.Models.Responses;
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
        public AsyncEvent<RawEventReceivedEventArgs> RawEventReceivedEvent { get; } = new();
        public AsyncEvent<RawResponseReceivedEventArgs> RawResponseReceivedEvent { get; } = new();
        public AsyncEvent<HttpContextEventArgs> HttpRequestFailedEvent { get; } = new();
    }
    
    public sealed class HttpContextEventArgs(RestRequest request, RestResponse? response) : EventArgs
    {
        public RestRequest Request { get; } = request;
        public RestResponse? Response { get; } = response;
    }

    public sealed class UserCredentialsReceivedEventArgs(UserCredentials userCredentials) : EventArgs
    {
        public UserCredentials UserCredentials { get; } = userCredentials;
    }
    
    
    public sealed class SessionIdReceivedEventArgs(string sessionId) : EventArgs
    {
        public string SessionId { get; } = sessionId;
    }


    public sealed class PermissionDeniedEventArgs(int callbackId, string commandId) : EventArgs
    {
        public int CallbackId { get; } = callbackId;
        public string CommandId { get; } = commandId;
    }

    public sealed class EventReceivedEventArgs(string eventName, string printer, IEventData? repetierEvent) : EventArgs
    {
        public IEventData? RepetierEvent { get; } = repetierEvent;
        public string EventName { get; } = eventName;
        public string Printer { get; } = printer;
    }

    public sealed class RawEventReceivedEventArgs : EventArgs
    {
        /// <summary>
        /// </summary>
        /// <param name="eventName"> Name of the received event </param>
        /// <param name="printer"> Printer associated with the event or empty if global </param>
        /// <param name="eventPayload"> Event payload as byte array </param>
        public RawEventReceivedEventArgs(string eventName, string printer, byte[] eventPayload)
        {
            EventName = eventName;
            Printer = printer;
            EventPayload = eventPayload;
        }

        public byte[] EventPayload { get; }
        public string EventName { get; }
        public string Printer { get; }
    }

    public sealed class ResponseReceivedEventArgs(RepetierResponse response) : EventArgs
    {
        public RepetierResponse Response { get; } = response;
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


    public sealed class LoginResultEventArgs(LoginResponse loginResult) : EventArgs
    {

        public LoginResponse LoginResult { get; } = loginResult;
    }

    public sealed class LoginRequiredEventArgs : EventArgs
    {
    }

    public sealed class DisconnectedEventArgs(DisconnectionInfo info) : EventArgs
    {
        public DisconnectionInfo Info { get; } = info;
    }

    public sealed class ConnectedEventArgs(Uri url, bool reconnect = false) : EventArgs
    {
        public Uri Url { get; } = url;
        public bool Reconnect { get; } = reconnect;

    }
}
