using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxiOnline.Toolkit.Events;

namespace TaxiOnline.Toolkit.Threading.Patterns
{
    /// <summary>
    /// класс для выполнения последовательных запросов
    /// </summary>
    /// <typeparam name="T">результат запроса</typeparam>
    public class SequentialRequestDecorator<T>
    {
        private readonly object _locker = new object();
        private volatile bool _isRequesting;
        
        /// <summary>
        /// запрос завершён
        /// </summary>
        public event EventHandler<ValueEventArgs<T>> RequestCompleted;

        /// <summary>
        /// выполнить запрос, если предыдущий был завершён
        /// </summary>
        /// <param name="requestDelegate">метод для выполнения запроса</param>
        public void Request(Func<T> requestDelegate)
        {
            lock (_locker)
            {
                if (_isRequesting)
                    return;
                _isRequesting = true;
                Task.Factory.StartNew(() => RunRequest(requestDelegate));
            }
        }

        private void RunRequest(Func<T> requestDelegate)
        {
            T outResult;
            try
            {
                outResult = requestDelegate();
            }
            finally
            {
                lock (_locker)
                    _isRequesting = false;
            }
            OnRequestCompleted(outResult);
        }

        protected virtual void OnRequestCompleted(T result)
        {
            EventHandler<ValueEventArgs<T>> handler = RequestCompleted;
            if (handler != null)
                handler(this, new ValueEventArgs<T>(result));
        }
    }
}
