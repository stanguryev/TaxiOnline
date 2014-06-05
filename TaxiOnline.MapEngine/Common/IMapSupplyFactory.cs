using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxiOnline.MapEngine.Cache;
using TaxiOnline.MapEngine.Composing;
using TaxiOnline.MapEngine.Geometry;
using TaxiOnline.MapEngine.Layers;

namespace TaxiOnline.MapEngine.Common
{
    public interface IMapSupplyFactory<TRawBitmap> where TRawBitmap : class
    {
        CacheBase<MapTile, TRawBitmap> GetTilesCache(int size, Func<MapTile, Task<TRawBitmap>> providerDelegate);
    }

    public interface IMapSupplyFactory<TRawBitmap, TBitmap> : IMapSupplyFactory<TRawBitmap>
        where TRawBitmap : class
        where TBitmap : class
    {
        ComposerBase<TRawBitmap, TBitmap> GetTilesComposer(TileLayerBase<TRawBitmap> tileLayer);
    }
}
