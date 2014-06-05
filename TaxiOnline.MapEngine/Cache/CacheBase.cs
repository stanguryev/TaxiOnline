using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TaxiOnline.Toolkit.Patterns;

namespace TaxiOnline.MapEngine.Cache
{
    public abstract class CacheBase<TKey, TItem> : DisposableObject
        where TKey : IEquatable<TKey>
        where TItem : class
    {
        private readonly Func<TKey, Task<TItem>> _providerDelegate;
        private EventWaitHandle _waitFetch = new EventWaitHandle(true, EventResetMode.AutoReset);

        public CacheBase(int size, Func<TKey, Task<TItem>> providerDelegate)
        {
            _providerDelegate = providerDelegate;
        }

        public abstract TItem GetCachedItem(TKey key);

        public abstract void PutCachedItem(TKey key, TItem cachedItem);

        public abstract void Clear();

        public async Task<TItem> GetItem(TKey key)
        {
            if (!_waitFetch.WaitOne(300))
                return null;
            TItem cachedItem;
            try
            {
                cachedItem = GetCachedItem(key);
                if (cachedItem == null)
                {
                    cachedItem = await _providerDelegate(key);
                    if (cachedItem != null)
                        PutCachedItem(key, cachedItem);
                }
            }
            finally
            {
                _waitFetch.Set();
            }
            return cachedItem;
        }
    }
}
