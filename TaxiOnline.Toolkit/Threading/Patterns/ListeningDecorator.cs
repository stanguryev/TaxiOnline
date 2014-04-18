using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TaxiOnline.Toolkit.Events;
using TaxiOnline.Toolkit.Patterns;

namespace TaxiOnline.Toolkit.Threading.Patterns
{
    /// <summary>
    /// класс для обработки в режиме ожидания сообщений (без уведомления). Передаёт обработку в другой поток
    /// </summary>
    /// <typeparam name="TMessage">тип сообщения</typeparam>
    public class ListeningDecorator<TMessage> : DisposableObject
    {
        private readonly Func<bool> _checkDelegate;
        private readonly Func<TMessage> _readDelegate;
        private readonly MessagesLoopDecorator<TMessage> _notificationDecorator;
        private readonly Task _listeningThread;
        private readonly Lazy<EventWaitHandle> _waitBlock = new Lazy<EventWaitHandle>(() => new EventWaitHandle(false, EventResetMode.AutoReset), true);
        private volatile bool _isActive = true;
        private TimeSpan _delay = TimeSpan.FromSeconds(0.5);
        private int? _blockSize;
        private int _numberInBlock;

        /// <summary>
        /// задрежка при отсутсвии сообщений
        /// </summary>
        public TimeSpan Delay
        {
            get { return _delay; }
            set { _delay = value; }
        }

        public int? BlockSize
        {
            get { return _blockSize; }
            set { _blockSize = value; }
        }

        /// <summary>
        /// сообщение получено (вызов из другого потока, чем считывание)
        /// </summary>
        public event EventHandler<ValueEventArgs<TMessage>> GotMessage;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="checkDelegate">функция проверки наличия сообщений</param>
        /// <param name="readDelegate">функция запроса сообщений</param>
        /// <param name="deferreStart">не начинать обработку при создании</param>
        public ListeningDecorator(Func<bool> checkDelegate, Func<TMessage> readDelegate, bool deferreStart)
        {
            _checkDelegate = checkDelegate;
            _readDelegate = readDelegate;
            _notificationDecorator = new MessagesLoopDecorator<TMessage>(OnGotMessage, deferreStart);
            _listeningThread = new Task(ListeningLoop);
            if (!deferreStart)
                _listeningThread.Start();
        }

        /// <summary>
        /// начать обработку при отложенном запуске
        /// </summary>
        public void Start()
        {
            _notificationDecorator.Start();
            _listeningThread.Start();
        }

        public void ResumeBlock()
        {
            _waitBlock.Value.Set();
        }

        protected override void DisposeManagedResources()
        {
            base.DisposeManagedResources();
            _isActive = false;
            _listeningThread.Wait();
            _notificationDecorator.Dispose();
        }

        private void ListeningLoop()
        {
            while (_isActive)
            {
                if (_checkDelegate())
                {
                    TMessage message = _readDelegate();
                    IDisposable safeUsageHandle = EnterSafeUsage();
                    if (safeUsageHandle == null)
                        return;
                    using (safeUsageHandle)
                        _notificationDecorator.PushMessage(message);
                    WaitBlock();
                }
                else
                    Task.Delay(_delay);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void WaitBlock()
        {
            int? blockSize = _blockSize;
            if (blockSize.HasValue)
            {
                _numberInBlock = (_numberInBlock + 1) % blockSize.Value;
                if (_numberInBlock == 0)
                    _waitBlock.Value.WaitOne();
            }
        }

        protected virtual void OnGotMessage(TMessage message)
        {
            EventHandler<ValueEventArgs<TMessage>> handler = GotMessage;
            if (handler != null)
                handler(this, new ValueEventArgs<TMessage>(message));
        }
    }
}
