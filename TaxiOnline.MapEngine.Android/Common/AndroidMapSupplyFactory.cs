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
using TaxiOnline.MapEngine.Common;
using TaxiOnline.MapEngine.Android.Composing;
using Android.Graphics;
using System.Threading.Tasks;

namespace TaxiOnline.MapEngine.Android.Common
{
    public class AndroidMapSupplyFactory : IMapSupplyFactory<byte[], Bitmap>
    {
        public MapEngine.Cache.CacheBase<Geometry.MapTile, byte[]> GetTilesCache(int size, Func<Geometry.MapTile, Task<byte[]>> providerDelegate)
        {
            return new Cache.LruCacheWrapper(size, tile => providerDelegate(tile));
        }

        public MapEngine.Composing.ComposerBase<byte[], Bitmap> GetTilesComposer(Layers.TileLayerBase<byte[]> tileLayer)
        {
            return new AndroidComposer(tileLayer);
        }
    }
}