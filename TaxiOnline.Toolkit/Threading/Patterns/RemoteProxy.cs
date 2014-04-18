using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TaxiOnline.Toolkit.Events;
using TaxiOnline.Toolkit.Patterns;
using TaxiOnline.Toolkit.Threading.Lock;

namespace TaxiOnline.Toolkit.Threading.Patterns
{
    /// <summary>
    /// класс предназначен для потокобезопасной работы с удалёнными соединениями
    /// </summary>
    /// <typeparam name="TChannel">тип данных соединения</typeparam>
    public abstract class RemoteProxy<TChannel> : DisposableObject where TChannel : class
    {
        protected enum ErrorType { SessionIsClosedError, FailedToReconnect, ConnectionWasChangedError, Timeout, IsConnecting, ConnectionIsClosingError }

        protected enum ConnectionCheckDecision { Use, Reconnect, NotifyError }

        protected TChannel _channel;
        private readonly EventWaitHandle _waitConnectionCompleted = new EventWaitHandle(false, EventResetMode.ManualReset);
        private readonly ReadWriteBox _connectionLocker = new ReadWriteBox();
        private readonly EventWaitHandle _waitCurrentReconnection = new EventWaitHandle(true, EventResetMode.AutoReset);
        private volatile bool _isConnecting;
        private ActionResult _lastConnectionResult;

        public TChannel Channel
        {
            get { return _channel; }
        }

        public event EventHandler<ValueEventArgs<CancellationTokenSource>> ReconnectionStarted;

        public event EventHandler ReconnectionFinished;

        public event EventHandler ReconnectionFinishing;

        public event EventHandler<ValueEventArgs<Exception>> ErrorOccured;

        protected RemoteProxy()
        {

        }

        public virtual void Abort()
        {
            _isDisposing = true;
            _isDisposed = true;
            TChannel channel = _channel;
            if (channel != null)
            {
                UnhookChannel(channel);
                AbortCore(channel);
            }
        }

        public ActionResult RunRequestSafe(Action requestDelegate, TChannel client, int? timeoutMilliseconds = null, CancellationToken? cancellation = null)
        {
            return RequestCore(requestDelegate, client, timeoutMilliseconds, cancellation);
        }

        public ActionResult<T> RunRequestSafe<T>(Func<T> requestDelegate, TChannel client, int? timeoutMilliseconds = null, CancellationToken? cancellation = null)
        {
            T result = default(T);
            ActionResult requestResult = RequestCore(() => result = requestDelegate(), client, timeoutMilliseconds, cancellation);
            if (requestResult.IsValid)
                return ActionResult<T>.GetValidResult(result);
            else
                return ActionResult<T>.GetErrorResult(requestResult);
        }

        public ActionResult WaitConnectionCompleted()
        {
            _waitConnectionCompleted.WaitOne();
            return _lastConnectionResult;
        }

        protected abstract TChannel CreateChannel();

        protected abstract void OpenChannel(TChannel channel);

        protected abstract ActionResult BuildErrorInfo(ErrorType errorType);

        protected abstract bool ProceedInvokeException(Exception exception, out ActionResult errorInfo);

        protected abstract bool ProceedConnectionException(Exception exception, out ActionResult errorInfo);

        protected abstract bool IsConnectionAvailable();

        protected abstract void NotifyConnectionStateChanged();

        protected abstract Tuple<ConnectionCheckDecision, ActionResult> CheckConnectionCore(TChannel channel);

        protected abstract void HookChannel(TChannel channel);

        protected abstract void UnhookChannel(TChannel channel);

        protected abstract void DisposeCore();

        protected abstract void AbortCore(TChannel channel);

        protected virtual void OnBeforeRequest()
        { }

        protected virtual void OnAfterRequest()
        { }

        protected override void DisposeManagedResources()
        {
            using (_connectionLocker.EnterWriteLock())
            {
                if (_channel == null)
                    return;
                DisposeCore();
            }
            _connectionLocker.Dispose();
        }

        protected void BeginConnect()
        {
            Task.Factory.StartNew(() =>
            {
                ActionResult result = TryConnect(notifyError: true);
                _lastConnectionResult = result;
                _waitConnectionCompleted.Set();
                Task.Factory.StartNew(NotifyConnectionStateChanged);
            });
        }

#if false

        protected async Task<ActionResult<TChannel>> ConnectAsync()
        {
            return await Task.Run(() =>
            {
                ActionResult result = TryConnect(notifyError: true);
                _lastConnectionResult = result;
                _waitConnectionCompleted.Set();
                TChannel channel = _channel;
                _notificationContext.Post(o => NotifyConnectionStateChanged(), null);
                return result.IsValid ? ActionResult<TChannel>.GetValidResult(channel) : ActionResult<TChannel>.GetErrorResult(result);
            });
        }

#endif

        protected void ProceedConnectionFaulted()
        {
            if (!_waitCurrentReconnection.WaitOne(1))
                return;
            CancellationTokenSource cancellation = new CancellationTokenSource();
            Task.Factory.StartNew(() => OnReconnectionStarted(cancellation));
            Task.Factory.StartNew(() => ReconnectOnFaulted(cancellation.Token, useCancellation: true));
            Task.Factory.StartNew(() => NotifyConnectionStateChanged());
        }

        private ActionResult TryConnect(bool isConnectionRequested = false, bool notifyError = false)
        {
            try
            {
                return Connect(isConnectionRequested);
            }
            catch (Exception ex)
            {
                if (notifyError)
                    NotifyError(ex);
                ActionResult errorInfo;
                if (!ProceedConnectionException(ex, out errorInfo))
                    throw;
                return errorInfo;
            }
        }

        private ActionResult Connect(bool isConnectionRequested)
        {
            object oldClient = _channel;
            if (_isConnecting)
                return BuildErrorInfo(ErrorType.IsConnecting);
            using (isConnectionRequested ? _connectionLocker.UpgradeToWriteLock() : _connectionLocker.EnterWriteLock())
            {
                if (_isDisposing)
                    return BuildErrorInfo(ErrorType.ConnectionIsClosingError);
                if (_isConnecting)
                    return BuildErrorInfo(ErrorType.IsConnecting);
                _isConnecting = true;
                if (_channel != null)
                    DisposeCore();
                try
                {
                    _channel = CreateChannel();
                    HookChannel(_channel);
                    OpenChannel(_channel);
                }
                finally
                {
                    _isConnecting = false;
                }
            }
            return ActionResult.ValidResult;
        }

        private ActionResult RequestCore(Action requestDelegate, TChannel channel, int? timeoutMilliseconds = null, CancellationToken? cancellation = null)
        {
            try
            {
                if (_isDisposed)
                    return BuildErrorInfo(ErrorType.SessionIsClosedError);
                using (_connectionLocker.EnterReadLock())
                {
                    if (channel == null || !IsConnectionAvailable())
                        return BuildErrorInfo(ErrorType.FailedToReconnect);
                    if (_channel != channel)
                        return BuildErrorInfo(ErrorType.ConnectionWasChangedError);
                    ActionResult connectionStateResult = CheckConnection(channel);
                    if (!connectionStateResult.IsValid)
                        return connectionStateResult;
                    ActionResult<object> invokeResult = InvokeWithTimeout<object>(() =>
                    {
                        OnBeforeRequest();
                        requestDelegate();
                        OnAfterRequest();
                        return null;
                    }, timeoutMilliseconds, cancellation);
                    if (!invokeResult.IsValid)
                        return ActionResult.GetErrorResult(invokeResult);
                }
                return ActionResult.ValidResult;
            }
            catch (Exception ex)
            {
                ActionResult errorInfo;
                if (!ProceedInvokeException(ex, out errorInfo))
                    throw;
                return errorInfo;
            }
        }

        private ActionResult CheckConnection(TChannel channel, int? timeoutMilliseconds = null)
        {
            if (IsDisposed)
                return BuildErrorInfo(ErrorType.SessionIsClosedError);
            Tuple<ConnectionCheckDecision, ActionResult> checkResult = CheckConnectionCore(channel);
            switch (checkResult.Item1)
            {
                case ConnectionCheckDecision.Use:
                    return ActionResult.ValidResult;
                case ConnectionCheckDecision.Reconnect:
                    return TryReconnect(isConnectionRequested: true, timeoutMilliseconds: timeoutMilliseconds);
                case ConnectionCheckDecision.NotifyError:
                    return checkResult.Item2;
                default:
                    throw new NotImplementedException();
            }

        }

        private ActionResult TryReconnect(bool isConnectionRequested, int? timeoutMilliseconds = null, CancellationToken? cancellation = null)
        {
            ActionResult<ActionResult> invokeResult = InvokeWithTimeout(() => TryConnect(isConnectionRequested), timeoutMilliseconds, cancellation);
            if (invokeResult.IsValid)
                return invokeResult.Result;
            else
                return ActionResult.GetErrorResult(invokeResult);
        }

        private ActionResult<T> InvokeWithTimeout<T>(Func<T> delegateToInvoke, int? timeoutMilliseconds, CancellationToken? cancellation)
        {
            T result;
            ActionResult<T> outResult = ActionResult<T>.GetErrorResult(""); ;
            if (timeoutMilliseconds == null && cancellation == null)
                outResult = ActionResult<T>.GetValidResult(delegateToInvoke());
            else if (timeoutMilliseconds != null && cancellation == null)
            {
                if (TimeoutHelper.InvokeWithTimeout<T>(delegateToInvoke, TimeSpan.FromMilliseconds(timeoutMilliseconds.Value), out result))
                    outResult = ActionResult<T>.GetValidResult(result);
                else
                    outResult = ActionResult<T>.GetErrorResult(BuildErrorInfo(ErrorType.Timeout));
            }
            else if (timeoutMilliseconds == null && cancellation != null)
            {
                if (TimeoutHelper.InvokeWithTimeout<T>(delegateToInvoke, cancellation.Value, out result))
                    outResult = ActionResult<T>.GetValidResult(result);
                else
                    outResult = ActionResult<T>.GetErrorResult(BuildErrorInfo(ErrorType.Timeout));
            }
            else if (timeoutMilliseconds != null && cancellation != null)
            {
                if (TimeoutHelper.InvokeWithTimeout<T>(delegateToInvoke, cancellation.Value, TimeSpan.FromMilliseconds(timeoutMilliseconds.Value), out result))
                    outResult = ActionResult<T>.GetValidResult(result);
                else
                    outResult = ActionResult<T>.GetErrorResult(BuildErrorInfo(ErrorType.Timeout));
            }
            return outResult;
        }

        private void ReconnectOnFaulted(CancellationToken cancellation, bool useCancellation)
        {
            TChannel channel = _channel;
            if (channel != null)
                UnhookChannel(channel);
            bool reconnectionResult;
            if (useCancellation)
                TimeoutHelper.InvokeWithTimeout(() => ReconnectLoop(cancellation, useCancellation), cancellation, out reconnectionResult);
            else
                Task.Factory.StartNew(() => ReconnectLoop(cancellation, useCancellation));
        }

        private bool ReconnectLoop(CancellationToken cancellation, bool useCancellation)
        {
            while (!TryConnect().IsValid && (!useCancellation || !cancellation.IsCancellationRequested))
            {
                Task.Delay(1000);
                if (_isDisposing)
                    return false;
            }
            OnReconnectionFinishing();
            Task.Factory.StartNew(OnReconnectionFinished);
            _waitCurrentReconnection.Set();
            return true;
        }

        private void NotifyError(Exception exception)
        {
            Task.Factory.StartNew(() => OnErrorOccured(exception));
        }

        protected virtual void OnReconnectionStarted(CancellationTokenSource cancellation)
        {
            EventHandler<ValueEventArgs<CancellationTokenSource>> handler = ReconnectionStarted;
            if (handler != null)
                handler(this, new ValueEventArgs<CancellationTokenSource>(cancellation));
        }

        protected virtual void OnReconnectionFinished()
        {
            EventHandler handler = ReconnectionFinished;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        protected virtual void OnReconnectionFinishing()
        {
            EventHandler handler = ReconnectionFinishing;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        protected virtual void OnErrorOccured(Exception exception)
        {
            EventHandler<ValueEventArgs<Exception>> handler = ErrorOccured;
            if (handler != null)
                handler(this, new ValueEventArgs<Exception>(exception));
        }

    }
}
