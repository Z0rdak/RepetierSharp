using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace RepetierSharp.Internal
{
    public sealed class AsyncEvent<TEventArgs> where TEventArgs : EventArgs
    {
        private readonly List<AsyncEventInvocator<TEventArgs>> _handlers = new();

        private ICollection<AsyncEventInvocator<TEventArgs>> _handlersForInvoke;

        public AsyncEvent()
        {
            _handlersForInvoke = _handlers;
        }

        // Track the existence of handlers in a separate field so that checking it all the time will not
        // require locking the actual list (_handlers).
        public bool HasHandlers { get; private set; }

        public void AddHandler(Func<TEventArgs, Task> handler)
        {
            if ( handler == null )
            {
                throw new ArgumentNullException(nameof(handler));
            }

            lock ( _handlers )
            {
                _handlers.Add(new AsyncEventInvocator<TEventArgs>(null, handler));

                HasHandlers = true;
                _handlersForInvoke = new List<AsyncEventInvocator<TEventArgs>>(_handlers);
            }
        }

        public void AddHandler(Action<TEventArgs> handler)
        {
            if ( handler == null )
            {
                throw new ArgumentNullException(nameof(handler));
            }

            lock ( _handlers )
            {
                _handlers.Add(new AsyncEventInvocator<TEventArgs>(handler, null));

                HasHandlers = true;
                _handlersForInvoke = new List<AsyncEventInvocator<TEventArgs>>(_handlers);
            }
        }

        public async Task InvokeAsync(TEventArgs eventArgs)
        {
            if ( !HasHandlers )
            {
                return;
            }

            // Adding or removing handlers will produce a new list instance all the time.
            // So locking here is not required since only the reference to an immutable list
            // of handlers is used.
            var handlers = _handlersForInvoke;
            foreach ( var handler in handlers )
            {
                await handler.InvokeAsync(eventArgs).ConfigureAwait(false);
            }
        }

        public void RemoveHandler(Func<TEventArgs, Task> handler)
        {
            if ( handler == null )
            {
                throw new ArgumentNullException(nameof(handler));
            }

            lock ( _handlers )
            {
                _handlers.RemoveAll(h => h.WrapsHandler(handler));

                HasHandlers = _handlers.Count > 0;
                _handlersForInvoke = new List<AsyncEventInvocator<TEventArgs>>(_handlers);
            }
        }

        public void RemoveHandler(Action<TEventArgs> handler)
        {
            if ( handler == null )
            {
                throw new ArgumentNullException(nameof(handler));
            }

            lock ( _handlers )
            {
                _handlers.RemoveAll(h => h.WrapsHandler(handler));

                HasHandlers = _handlers.Count > 0;
                _handlersForInvoke = new List<AsyncEventInvocator<TEventArgs>>(_handlers);
            }
        }

        public async Task TryInvokeAsync(TEventArgs eventArgs, ILogger logger)
        {
            if ( eventArgs == null )
            {
                throw new ArgumentNullException(nameof(eventArgs));
            }

            if ( logger == null )
            {
                throw new ArgumentNullException(nameof(logger));
            }

            try
            {
                await InvokeAsync(eventArgs).ConfigureAwait(false);
            }
            catch ( Exception exception )
            {
                logger.LogWarning(exception, "Error while invoking event with arguments of type {Type}.",
                    typeof(TEventArgs));
            }
        }
    }
}
