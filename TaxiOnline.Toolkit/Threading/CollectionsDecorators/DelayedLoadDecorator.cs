using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using System.Threading;
using TaxiOnline.Toolkit.Events;
using TaxiOnline.Toolkit.Patterns;
using System.Runtime.CompilerServices;

namespace TaxiOnline.Toolkit.Threading.CollectionsDecorators
{
    /// <summary>
    /// класс предназначен для отложенной асинхронной загрузки коллекции элементов. Позволяет проверить факт начала загрузки и выполнить загрузку в синхронном режиме
    /// </summary>
    /// <typeparam name="TItem">тип элемента коллекции</typeparam>
    public class DelayedLoadDecorator<TItem> : DisposableObject
    {
        private readonly Func<IEnumerable<TItem>> _requestDelegate;
        private readonly Func<ActionResult<IEnumerable<TItem>>> _wrappedRequestDelegate;
        protected ObservableCollection<TItem> _items;
        private readonly object _loadLocker = new object();
        private readonly EventWaitHandle _waitLoading = new EventWaitHandle(true, EventResetMode.ManualReset);
        private volatile bool _isLoading;
        private volatile bool _lastLoadFailed;

        /// <summary>
        /// получить коллекцию элементов
        /// </summary>
        public virtual ObservableCollection<TItem> Items
        {
            get { return _items; }
        }

        /// <summary>
        /// произведена замена коллекции элементов
        /// </summary>
        public event EventHandler ItemsChanged;

        /// <summary>
        /// произошла ошибка в асинхронном запросе
        /// </summary>
        public event EventHandler RequestFailed;

        /// <summary>
        /// позволяет получить данные об ошибке запроса
        /// </summary>
        public event EventHandler<ValueEventArgs<Exception>> ExceptionOccurred;

        /// <summary>
        /// создать в режиме внутреннего перехватывания исключений
        /// </summary>
        /// <param name="requestDelegate">функция для запроса коллекции, вызываемая асинхронно по запросу</param>
        public DelayedLoadDecorator(Func<IEnumerable<TItem>> requestDelegate)
        {
            _requestDelegate = requestDelegate;
        }

        /// <summary>
        /// создать в режиме с перехватыванием исключений в вызываемом методе
        /// </summary>
        /// <param name="wrappedRequestDelegate">функция для запроса коллекции, вызываемая асинхронно по запросу</param>
        public DelayedLoadDecorator(Func<ActionResult<IEnumerable<TItem>>> wrappedRequestDelegate)
        {
            _wrappedRequestDelegate = wrappedRequestDelegate;
        }

        /// <summary>
        /// проверить, была ли начата загрузка, начать, если загрузка ещё не началась
        /// рекомендуется вызывать при каждом действии, при котором выполняется отложенная загрузка
        /// </summary>
        public void EnsureInitialization()
        {
            FillListAsync(() => !_isLoading && _items == null);
        }

        /// <summary>
        /// повторить загрузку элементов, если загрузка не выполняется
        /// </summary>
        /// <returns>поток, в котором выполняется загрузка</returns>
        public Task Reload()
        {
            return FillListAsync(() => !_isLoading);
        }

        /// <summary>
        /// выполнить загрузку в синхронном режиме. Может возникнуть длительное ожидание уже начатой загрузки
        /// </summary>
        /// <returns>выполнена ли загрука</returns>
        public bool EnsureInitializationSync()
        {
            bool outResult = true;
            bool needsWait = false;
            IDisposable safeUsageOperation = EnterSafeUsage();
            if (safeUsageOperation == null)
                return false;
            using (safeUsageOperation)
            {
                lock (_loadLocker)
                {
                    if (!_isLoading && _items == null)
                    {
                        _isLoading = true;
                        _waitLoading.Reset();
                    }
                    else
                        needsWait = true;
                }
                if (needsWait)
                {
                    _waitLoading.WaitOne();
                    return !_lastLoadFailed;
                }
            }
            _lastLoadFailed = false;
            ActionResult fillResult = InvokeFillListCoreWrapped();
            if (!fillResult.IsValid)
            {
                OnExceptionOccurred(fillResult.FailException);
                _lastLoadFailed = true;
                outResult = false;
            }
            ResetIsLoading();
            if (outResult)
                OnItemsChanged();
            else
                OnRequestFailed();
            return outResult;
        }

        protected virtual void OnItemsChanged()
        {
            EventHandler handler = ItemsChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        protected virtual void OnRequestFailed()
        {
            EventHandler handler = RequestFailed;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        protected virtual void OnExceptionOccurred(Exception exception)
        {
            EventHandler<ValueEventArgs<Exception>> handler = ExceptionOccurred;
            if (handler != null)
                handler(this, new ValueEventArgs<Exception>(exception));
        }

        protected override void DisposeUnmanagedResources()
        {
            base.DisposeUnmanagedResources();
            _waitLoading.Dispose();
        }

        protected virtual void FillListCore()
        {
            _items = new ObservableCollection<TItem>(_requestDelegate());
        }

        protected virtual ActionResult WrappedFillListCore()
        {
            ActionResult<IEnumerable<TItem>> requestResult = _wrappedRequestDelegate();
            if (requestResult.IsValid)
            {
                _items = new ObservableCollection<TItem>(requestResult.Result);
                return ActionResult.ValidResult;
            }
            else
                return ActionResult.GetErrorResult(requestResult);
        }

        private ActionResult InvokeFillListCoreWrapped()
        {
            if (_wrappedRequestDelegate != null)
                return WrappedFillListCore();
            else
            {
                try
                {
                    FillListCore();
                }
                catch (Exception ex)
                {
                    OnExceptionOccurred(ex);
                    return ActionResult.GetErrorResult(ex);
                }
                return ActionResult.ValidResult;
            }
        }
        
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private Task FillListAsync(Func<bool> condition = null)
        {
            IDisposable safeUsageOperation = EnterSafeUsage();
            if (safeUsageOperation == null)
                return null;
            using (safeUsageOperation)
                lock (_loadLocker)
                {
                    if (condition == null || condition())
                    {
                        _isLoading = true;
                        _waitLoading.Set();
                        _lastLoadFailed = false;
                        return Task.Factory.StartNew(FillList);
                    }
                    else
                        return null;
                }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void FillList()
        {
            ActionResult fillResult = InvokeFillListCoreWrapped();
            if (!fillResult.IsValid)
            {
                OnExceptionOccurred(fillResult.FailException);
                _lastLoadFailed = true;
                ResetIsLoading();
                OnRequestFailed();
                return;
            }
            //try
            //{
            //    FillListCore();
            //}
            //catch (Exception ex)
            //{
            //    OnExceptionOccurred(ex);
            //    _lastLoadFailed = true;
            //    ResetIsLoading();
            //    OnRequestFailed();
            //    return;
            //}
            ResetIsLoading();
            OnItemsChanged();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ResetIsLoading()
        {
            IDisposable safeUsageOperation = EnterSafeUsage();
            if (safeUsageOperation == null)
                return;
            using (safeUsageOperation)
                lock (_loadLocker)
                {
                    _isLoading = false;
                    _waitLoading.Set();
                }
        }
    }
}