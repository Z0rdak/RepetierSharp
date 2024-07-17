using System;
using System.Threading.Tasks;

namespace RepetierSharp.Internal
{
    public readonly struct AsyncEventInvocator<TEventArgs>
    {
        private readonly Action<TEventArgs> _handler;
        private readonly Func<TEventArgs, Task> _asyncHandler;

        public AsyncEventInvocator(Action<TEventArgs> handler, Func<TEventArgs, Task> asyncHandler)
        {
            _handler = handler;
            _asyncHandler = asyncHandler;
        }

        public bool WrapsHandler(Action<TEventArgs> handler)
        {
            // Do not use ReferenceEquals! It will not work with delegates.
            return handler == _handler;
        }

        public bool WrapsHandler(Func<TEventArgs, Task> handler)
        {
            // Do not use ReferenceEquals! It will not work with delegates.
            return handler == _asyncHandler;
        }

        public Task InvokeAsync(TEventArgs eventArgs)
        {
            if ( _handler != null )
            {
                _handler.Invoke(eventArgs);
                return Task.CompletedTask;
            }

            if ( _asyncHandler != null )
            {
                return _asyncHandler.Invoke(eventArgs);
            }

            throw new InvalidOperationException();
        }
    }
}
