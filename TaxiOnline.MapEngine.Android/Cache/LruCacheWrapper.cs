using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.Util;
using TaxiOnline.MapEngine.Cache;
using TaxiOnline.MapEngine.Geometry;
using System.Threading.Tasks;

namespace TaxiOnline.MapEngine.Android.Cache
{
    public class LruCacheWrapper : CacheBase<MapTile, byte[]>
    {
        private readonly LruCache _internalCache;

        public LruCacheWrapper(int size, Func<MapTile, Task<byte[]>> providerDelegate)
            : base(size, providerDelegate)
        {
            _internalCache = new LruCache(size);
        }

        public override byte[] GetCachedItem(MapTile key)
        {
            DataWrapper<byte[]> outResult = (DataWrapper<byte[]>)_internalCache.Get(key.ToString());
            return outResult == null ? null : outResult.Data;
        }

        public override void PutCachedItem(MapTile key, byte[] cachedItem)
        {
            _internalCache.Put(key.ToString(), new DataWrapper<byte[]>(cachedItem));
        }

        public override void Clear()
        {
            _internalCache.EvictAll();
        }

        protected override void DisposeManagedResources()
        {
            base.DisposeManagedResources();
            _internalCache.Dispose();
        }
    }
}