using System;

namespace RepetierSharp.Internal
{
    public abstract class Disposable : IDisposable
    {
        protected bool IsDisposed { get; private set; }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            if ( IsDisposed )
            {
                return;
            }

            IsDisposed = true;

            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected void ThrowIfDisposed()
        {
            if ( IsDisposed )
            {
                throw new ObjectDisposedException(GetType().Name);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
        }
    }
}
