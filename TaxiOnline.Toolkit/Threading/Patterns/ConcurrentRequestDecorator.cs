using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace TaxiOnline.Toolkit.Threading.Patterns
{
    public class ConcurrentRequestDecorator<TData>
    {
        private readonly Func<TData> _requestDelegate;
        private readonly EventWaitHandle _waitRequest = new EventWaitHandle(true, EventResetMode.AutoReset);
        private bool _isReady;
        private TData _value;

        public bool IsReady
        {
            get { return _isReady; }
            set { _isReady = value; }
        }

        public TData Value
        {
            get { return GetValue(); }
        }

        public ConcurrentRequestDecorator(Func<TData> requestDelegate, TData initialValue, bool isReadyInitially)
        {
            _requestDelegate = requestDelegate;
            _value = initialValue;
            _isReady = isReadyInitially;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private TData GetValue()
        {
            if (!_isReady)
            {
                _waitRequest.WaitOne();
                try
                {
                    _value = _requestDelegate();
                    _isReady = true;
                }
                finally
                {
                    _waitRequest.Set();
                }
            }
            return _value;
        }
    }
}
