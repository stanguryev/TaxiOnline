using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxiOnline.MapEngine.Common;
using TaxiOnline.MapEngine.Providers;

namespace TaxiOnline.MapEngine.Layers
{
    public class HttpTileLayer<TBitmap> : TileLayerBase<TBitmap> where TBitmap : class
    {
        public HttpTileLayer(IMapSupplyFactory<TBitmap> supplyFactory, string urlTemplate)
            : base(supplyFactory, (TileProviderBase<TBitmap>)(object)new HttpTileProvider(urlTemplate))
        {

        }
    }
}
