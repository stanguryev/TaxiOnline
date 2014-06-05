using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;

namespace TaxiOnline.MapEngine.Geometry
{
    public class ObservableMapTileSet : MapTileSet<ObservableCollection<MapTile>>
    {
        public event NotifyCollectionChangedEventHandler TilesCollectionChanged
        {
            add { _collection.CollectionChanged += value; }
            remove { _collection.CollectionChanged -= value; }
        }

        public ObservableMapTileSet(double centerLatitude, double centerLongitude, double zoom)
            : base(centerLatitude, centerLongitude, zoom)
        {

        }

        protected override ObservableCollection<MapTile> CreateCollection(IEnumerable<MapTile> data)
        {
            return new ObservableCollection<MapTile>(data);
        }
    }
}
