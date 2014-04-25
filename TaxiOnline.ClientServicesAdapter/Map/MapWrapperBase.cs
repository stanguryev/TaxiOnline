using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxiOnline.ClientInfrastructure.Data;
using TaxiOnline.ClientInfrastructure.ServicesEntities.Map;

namespace TaxiOnline.ClientServicesAdapter.Map
{
    public abstract class MapWrapperBase : IMap
    {
        protected readonly OsmSharp.UI.Map.Map _map;
        protected MapPoint _mapCenter;
        protected double _mapZoom;

        public MapPoint MapCenter
        {
            get { return _mapCenter; }
            set
            {
                _mapCenter = value;
                SetMapCenter(value);
            }
        }

        public double MapZoom
        {
            get { return _mapZoom; }
            set
            {
                _mapZoom = value;
                SetMapZoom(value);
            }
        }

        public MapWrapperBase()
        {
            _map = new OsmSharp.UI.Map.Map();
            _map.AddLayer(new OsmSharp.UI.Map.Layers.LayerTile(@"http://otile1.mqcdn.com/tiles/1.0.0/osm/{0}/{1}/{2}.png"));
            _map.AddLayer(new OsmSharp.UI.Map.Layers.LayerTile(@"http://tiles.openseamap.org/seamark/{0}/{1}/{2}.png"));
        }

        public int LatitudeOffsetToPixels(double from, double to, double longitude)
        {
            OsmSharp.Units.Distance.Meter distance = new OsmSharp.Math.Geo.GeoCoordinate(from, longitude).DistanceEstimate(new OsmSharp.Math.Geo.GeoCoordinate(to, longitude));
            return (int)(distance.Value / 39800000.0 * 256.0 * Math.Pow(2.0, _mapZoom - 1));
        }

        public int LongitudeOffsetToPixels(double from, double to, double latitude)
        {
            OsmSharp.Units.Distance.Meter distance = new OsmSharp.Math.Geo.GeoCoordinate(latitude, from).DistanceEstimate(new OsmSharp.Math.Geo.GeoCoordinate(latitude, to));
            return (int)(distance.Value / 39800000.0 * 256.0 * Math.Pow(2.0, _mapZoom - 1));
        }

        protected abstract void SetMapCenter(MapPoint value);

        protected abstract void SetMapZoom(double value);
    }
}
