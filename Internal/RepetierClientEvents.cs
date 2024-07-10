using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using RepetierSharp.Models.Events;
using RepetierSharp.Models.Messages;

namespace RepetierSharp.Internal
{
    public sealed class RepetierClientEvents
    {
        public AsyncEvent<RepetierConnectedEventArgs> ConnectedEvent { get; } = new AsyncEvent<RepetierConnectedEventArgs>();
        public AsyncEvent<LoginRequiredEventArgs> LoginRequiredEvent = new AsyncEvent<LoginRequiredEventArgs>();
        public AsyncEvent<LoginResultEventArgs> LoginResultEvent = new AsyncEvent<LoginResultEventArgs>();
        public AsyncEvent<RepetierEventReceivedEventArgs> RepetierEventReceivedEvent = new AsyncEvent<RepetierEventReceivedEventArgs>();
        public AsyncEvent<RepetierResponseReceivedEventArgs> RepetierResponseReceivedEvent = new AsyncEvent<RepetierResponseReceivedEventArgs>();
        public AsyncEvent<RawRepetierEventReceivedEventArgs> RawRepetierEventReceivedEvent = new AsyncEvent<RawRepetierEventReceivedEventArgs>();
        public AsyncEvent<RawRepetierResponseReceivedEventArgs> RawRepetierResponseReceivedEvent = new AsyncEvent<RawRepetierResponseReceivedEventArgs>();

        /// <summary>
        /// Event for received events from the repetier server.
        /// </summary>
        public sealed class RepetierEventReceivedEventArgs : EventArgs
        {
            public RepetierEventReceivedEventArgs(string eventName, string printer, IRepetierEvent? repetierEvent)
            {
                EventName = eventName;
                Printer = printer;
                RepetierEvent = repetierEvent;
            }

            public IRepetierEvent? RepetierEvent { get; set; }
            public string EventName { get; set; }
            public string Printer { get; set; }
        }
        
        /// <summary>
        /// Fired whenever an event from the repetier server is received. 
        /// The payload is the raw event itself (content of the data field of the json from the documentation).
        /// </summary>
        public sealed class RawRepetierEventReceivedEventArgs : EventArgs
        {
            /// <summary>
            /// 
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

            public byte[] EventPayload { get; set; }
            public string EventName { get; set; }
            public string Printer { get; set; }
        }
        
        public sealed class RepetierResponseReceivedEventArgs : EventArgs
        {
            public RepetierResponseReceivedEventArgs(int callbackId, string command, IRepetierMessage? message)
            {
                CallbackId = callbackId;
                Command = command;
                Message = message;
            }

            public string Command { get; set; }
            public int CallbackId { get; set; }
            public IRepetierMessage? Message { get; set; }
        }
        
        
        /// <summary>
        /// Fired whenever a command response from the repetier server is received. 
        /// The payload is the raw response itself (content of the data field of the json from the documentation).
        /// </summary>
        public sealed class RawRepetierResponseReceivedEventArgs : EventArgs
        {
            /// <summary>
            /// 
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

            public string Command { get; set; }
            public int CallbackId { get; set; }
            public byte[] ResponsePayload { get; set; }
        }
    }

    
    
    public sealed class LoginResultEventArgs : EventArgs
    {
        public LoginResultEventArgs(LoginResponse loginResult)
        {
            LoginResult = loginResult;
        }

        public LoginResponse LoginResult { get; set; }
    }
    
    public sealed class LoginRequiredEventArgs : EventArgs
    {
        public LoginRequiredEventArgs()
        {
        }
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
