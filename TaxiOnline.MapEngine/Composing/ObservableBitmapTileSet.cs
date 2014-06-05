using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxiOnline.ClientInfrastructure.Data;
using TaxiOnline.MapEngine.Geometry;

namespace TaxiOnline.MapEngine.Composing
{
    public class ObservableBitmapTileSet : BitmapTileSet<ObservableCollection<MapTile>, ObservableCollection<BitmapTile>>
    {
        public event NotifyCollectionChangedEventHandler BitmapsCollectionChanged
        {
            add { _collection.CollectionChanged += value; }
            remove { _collection.CollectionChanged -= value; }
        }

        public ObservableBitmapTileSet(MapPoint center, double zoom, BitmapSize totalSize, BitmapSize tileSize, ObservableMapTileSet tileSet)
            : base(center, zoom, totalSize, tileSize, tileSet)
        {

        }

        protected override ObservableCollection<BitmapTile> CreateCollection(IEnumerable<BitmapTile> enumeration)
        {
            return new ObservableCollection<BitmapTile>(enumeration);
        }
    }
}
