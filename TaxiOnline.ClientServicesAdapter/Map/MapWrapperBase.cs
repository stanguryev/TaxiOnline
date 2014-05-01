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
        private const double Epsilon = 1e-5;

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

        internal OsmSharp.UI.Map.Map Map
        {
            get { return _map; }
        }

        public event EventHandler MapChanged;

        public event EventHandler MapCenterChanged;

        public event EventHandler MapZoomChanged;

        public MapWrapperBase()
        {
            _map = new OsmSharp.UI.Map.Map();
            _map.MapChanged += Map_MapChanged;
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

        protected abstract MapPoint GetMapCenter();

        protected abstract double GetMapZoom();

        private void Map_MapChanged()
        {
            MapPoint mapCenter = GetMapCenter();
            double mapZoom = GetMapZoom();
            if (Math.Abs(_mapCenter.Latitude - mapCenter.Latitude) + Math.Abs(_mapCenter.Longitude - mapCenter.Longitude) > Epsilon)
            {
                _mapCenter = mapCenter;
                OnMapCenterChanged();
            }
            if (Math.Abs(_mapZoom - mapZoom) > Epsilon)
            {
                _mapZoom = mapZoom;
                OnMapZoomChanged();
            }
            OnMapChanged();
        }

        protected virtual void OnMapChanged()
        {
            EventHandler handler = MapChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        protected virtual void OnMapCenterChanged()
        {
            EventHandler handler = MapCenterChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        protected virtual void OnMapZoomChanged()
        {
            EventHandler handler = MapZoomChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }
    }
}
