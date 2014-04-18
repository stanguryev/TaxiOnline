using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;

namespace TaxiOnline.Toolkit.Threading.Lock
{
    /// <summary>
    /// класс предназначен для раздельной блокировки на чтение и на запись
    /// рекомендуется применять в ситуации, когда одновременная обработка нескольких запросов на чтение может повысить быстродействие по сравнению с полной блокировкой каждого доступа
    /// класс реализует предотвращение бесконечного откладывания, после запроса блокировки на запись обработка новых запросов на чтение приостанавливается до завершения записи
    /// </summary>
    public class ReadWriteBox : IDisposable
    {
        private enum OperationType { Read, UpgradeableRead, Write }

        private class ReadWriteOperation : IDisposable
        {
            private readonly ReaderWriterLockSlim _locker;
            private readonly OperationType _opertation;

            public ReadWriteOperation(ReaderWriterLockSlim locker, OperationType opertation)
            {
                _locker = locker;
                _opertation = opertation;
                switch (opertation)
                {
                    case OperationType.Read:
                        locker.EnterReadLock();
                        break;
                    case OperationType.UpgradeableRead:
                        locker.EnterUpgradeableReadLock();
                        break;
                    case OperationType.Write:
                        locker.EnterWriteLock();
                        break;
                    default:
                        break;
                }
            }

            public void Dispose()
            {

                switch (_opertation)
                {
                    case OperationType.Read:
                        _locker.ExitReadLock();
                        break;
                    case OperationType.UpgradeableRead:
                        _locker.ExitUpgradeableReadLock();
                        break;
                    case OperationType.Write:
                        _locker.ExitWriteLock();
                        break;
                    default:
                        break;
                }
            }
        }

        private class UpgradeDowngradeOperation : IDisposable
        {
            private readonly ReaderWriterLockSlim _locker;

            public UpgradeDowngradeOperation(ReaderWriterLockSlim locker)
            {
                _locker = locker;
                _locker.EnterWriteLock();
            }

            public void Dispose()
            {
                _locker.ExitWriteLock();
            }
        }

        private readonly ReaderWriterLockSlim _locker = new ReaderWriterLockSlim();
        private readonly EventWaitHandle _waitWrite;

        public ReadWriteBox(bool preventWriteDelay = true)
        {
            if (preventWriteDelay)
                _waitWrite = new EventWaitHandle(true, EventResetMode.ManualReset);
        }

        /// <summary>
        /// обеспечивает вход в блокировку на чтение до вызова метода Dispose. Рекомендуется применять с оператором using
        /// </summary>
        /// <returns>интерфейс, позволяющий вызвать метод Dispose для выхода из блокировки</returns>
        /// <exception cref="System.ApplicationException">при длительном ожидании входа, вызвавшем сбой. Скорее всего означает, что возник deadlock</exception>
        public IDisposable EnterReadLock()
        {
            if (_waitWrite != null)
                _waitWrite.WaitOne();
            return new ReadWriteOperation(_locker, OperationType.Read);
        }
        
        public IDisposable EnterUpgradeableReadLock()
        {
            if (_waitWrite != null)
                _waitWrite.WaitOne();
            return new ReadWriteOperation(_locker, OperationType.UpgradeableRead);
        }

        /// <summary>
        /// обеспечивает вход в блокировку на запись до вызова метода Dispose. Рекомендуется применять с оператором using
        /// </summary>
        /// <returns>интерфейс, позволяющий вызвать метод Dispose для выхода из блокировки</returns>
        /// <exception cref="System.ApplicationException">при длительном ожидании входа, вызвавшем сбой. Скорее всего означает, что возник deadlock</exception>
        public IDisposable EnterWriteLock()
        {
            return InvokePreventingDelay<IDisposable>(() => new ReadWriteOperation(_locker, OperationType.Write));
        }

        public IDisposable UpgradeToWriteLock()
        {
            return InvokePreventingDelay<IDisposable>(() => new UpgradeDowngradeOperation(_locker));
        }

        /// <summary>
        /// освободить системные ресурсы, использованные для управления блокировками
        /// </summary>
        public void Dispose()
        {
            if (_waitWrite != null)
                _waitWrite.Dispose();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private T InvokePreventingDelay<T>(Func<T> functionToInvoke)
        {
            T outResult;
            if (_waitWrite != null)
            {
                _waitWrite.Reset();
                try
                {
                    outResult = functionToInvoke();
                }
                finally
                {
                    _waitWrite.Set();
                }
            }
            else
                outResult = functionToInvoke();
            return outResult;
        }
    }
}
