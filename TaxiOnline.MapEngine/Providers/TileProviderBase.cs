using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TaxiOnline.MapEngine.Providers
{
    public abstract class TileProviderBase<TBitmap>
    {
        public abstract Task<TBitmap> GetBitmap(Geometry.MapTile tile);
    }
}
