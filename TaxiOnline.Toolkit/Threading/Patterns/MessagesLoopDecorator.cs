using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using System.Threading.Tasks;
using TaxiOnline.Toolkit.Patterns;

namespace TaxiOnline.Toolkit.Threading.Patterns
{
    /// <summary>
    /// класс для обработки входящих сообщений по очереди с ожиданием заполнения
    /// </summary>
    /// <typeparam name="TMessage">тип сообщения</typeparam>
    public class MessagesLoopDecorator<TMessage> : DisposableObject
    {
        private readonly Action<TMessage> _loopDelegate;
        private readonly Queue<TMessage> _messages = new Queue<TMessage>();
        private readonly EventWaitHandle _waitIncoming = new EventWaitHandle(true, EventResetMode.AutoReset);
        private readonly Task _loopThread;
        private volatile bool _isActive = true;

        /// <summary>
        /// переход в режим ожидания новых сообщений
        /// </summary>
        public event EventHandler GoingToAwait;

        /// <summary>
        /// возобновление обработки при поступлении новых сообщений
        /// </summary>
        public event EventHandler Reactivating;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="loopDelegate">действие по обработке сообщения</param>
        /// <param name="deferreStart">не начинать обработку при создании</param>
        public MessagesLoopDecorator(Action<TMessage> loopDelegate, bool deferreStart)
        {
            _loopDelegate = loopDelegate;
            _loopThread = new Task(Loop);
            if (!deferreStart)
                _loopThread.Start();
        }

        /// <summary>
        /// начать обработку в режиме отложенного запуска
        /// </summary>
        public void Start()
        {
            _loopThread.Start();
        }

        /// <summary>
        /// добавить сообщение в очередь
        /// </summary>
        /// <param name="message">сообщение</param>
        public void PushMessage(TMessage message)
        {
            if (_isDisposed || _isDisposing)
                return;
            PushMessageCore(message);
        }

        /// <summary>
        /// добавить несколько сообщений в очередь
        /// </summary>
        /// <param name="messages"></param>
        public void PushMessageRange(IEnumerable<TMessage> messages)
        {
            if (_isDisposed || _isDisposing)
                return;
            PushMessageCore(messages.ToArray());
        }

        /// <summary>
        /// завершение обработки
        /// </summary>
        protected override void DisposeManagedResources()
        {
            base.DisposeManagedResources();
            StopCore();
            _waitIncoming.Dispose();
        }

        /// <summary>
        /// цикл обработки
        /// </summary>
        private void Loop()
        {
            while (_isActive)
            {
                OnReactivating();
                while (_messages.Count > 0)
                    _loopDelegate(_messages.Dequeue());
                if (_isActive)
                {
                    IDisposable safeUsageHandle = EnterSafeUsage();
                    if (safeUsageHandle == null)
                        return;
                    OnGoingToAwait();
                    using (safeUsageHandle)
                        if (!_isDisposing && !_isDisposed)
                            _waitIncoming.WaitOne();
                }
            }
        }

        /// <summary>
        /// функция добавления сообщений в очередь
        /// </summary>
        /// <param name="messages">сообщения</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void PushMessageCore(params TMessage[] messages)
        {
            foreach (TMessage message in messages)
                _messages.Enqueue(message);
            IDisposable safeUsageHandle = EnterSafeUsage();
            if (safeUsageHandle == null)
                return;
            using (safeUsageHandle)
                _waitIncoming.Set();
        }

        /// <summary>
        /// фукнция прекращения обработки
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void StopCore()
        {
            _isActive = false;
            //IDisposable safeUsageHandle = EnterSafeUsage();
            //if (safeUsageHandle == null)
            //    return;
            //using (safeUsageHandle)
            _waitIncoming.Set();
            _loopThread.Wait();
        }

        protected virtual void OnGoingToAwait()
        {
            EventHandler handler = GoingToAwait;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        protected virtual void OnReactivating()
        {
            EventHandler handler = Reactivating;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }
    }
}
