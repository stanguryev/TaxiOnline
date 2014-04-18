using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics;
using System.Threading;
using System.Runtime.CompilerServices;

namespace TaxiOnline.Toolkit.Patterns
{
    public class DisposableObject : IDisposable
    {
        private class UsageOperation : IDisposable
        {
            private readonly EventWaitHandle _waitHandle;

            public UsageOperation(EventWaitHandle waitHandle)
            {
                waitHandle.Reset();
                _waitHandle = waitHandle;
            }

            public void Dispose()
            {
                _waitHandle.Set();
                GC.SuppressFinalize(this);
            }
        }

        protected volatile bool _isDisposed = false;
        protected volatile bool _isDisposing;
        protected readonly object _disposeLocker = new object();
        private volatile EventWaitHandle _waitBeforeDispose;

        public bool IsDisposed
        {
            get { return _isDisposed; }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~DisposableObject()
        {
            Debug.Assert(_isDisposed, "Освобождение ресурсов следует выполнять упорядоченно");
            Dispose(false);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IDisposable EnterSafeUsage()
        {
            if (_isDisposed || _isDisposing)
                return null;
            lock (_disposeLocker)
            {
                if (_isDisposed || _isDisposing)
                    return null;
                if (_waitBeforeDispose == null)
                    _waitBeforeDispose = new EventWaitHandle(true, EventResetMode.ManualReset);
                return new UsageOperation(_waitBeforeDispose);
            }
        }

        private void Dispose(bool disposing)
        {
            if (_isDisposed || _isDisposing)
                return;
            lock (_disposeLocker)
                if (!_isDisposed && !_isDisposing)
                {
                    _isDisposing = true;
                    if (_waitBeforeDispose != null)
                        _waitBeforeDispose.WaitOne();
                    if (disposing)
                        DisposeManagedResources();
                    DisposeUnmanagedResources();
                    _isDisposed = true;
                }
        }

        protected virtual void DisposeManagedResources() { }

        protected virtual void DisposeUnmanagedResources() { }
    }
}
