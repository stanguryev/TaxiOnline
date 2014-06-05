using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxiOnline.ClientInfrastructure.Data;
using TaxiOnline.ClientInfrastructure.ServicesEntities.Map;
using TaxiOnline.MapEngine.Common;
using TaxiOnline.MapEngine.Composing;

namespace TaxiOnline.ClientServicesAdapter.Map
{
    public abstract class MapWrapperBase : IMap
    {
        private const double Epsilon = 1e-5;

        protected readonly MapBase _map;
        protected MapPoint _mapCenter;
        protected double _mapZoom;

        public MapPoint MapCenter
        {
            get { return _map.Center; }
            set { _map.Center = value; }
        }

        public double MapZoom
        {
            get { return _map.Zoom; }
            set { _map.Zoom = value; }
        }

        internal MapBase Map
        {
            get { return _map; }
        }

        public event EventHandler MapChanged
        {
            add { _map.TotalSizeChanged += value; }
            remove { _map.TotalSizeChanged -= value; }
        }

        public event EventHandler MapCenterChanged
        {
            add { _map.CenterChanged += value; }
            remove { _map.CenterChanged -= value; }
        }

        public event EventHandler MapZoomChanged
        {
            add { _map.ZoomChanged += value; }
            remove { _map.ZoomChanged -= value; }
        }

        public MapWrapperBase()
        {
            _map = CreateMap();
        }

        public bool GetPixelsFromCoordinates(MapPoint coordinates, out int x, out int y)
        {
            BitmapSize? offset = _map.GetOffsetFromCoordinates(coordinates);
            if (offset.HasValue)
            {
                x = offset.Value.Width;
                y = offset.Value.Height;
            }
            else
                x = y = 0;
            return offset.HasValue;
        }

        //public int LatitudeOffsetToPixels(double from, double to, double longitude)
        //{
        //    OsmSharp.Units.Distance.Meter distance = new OsmSharp.Math.Geo.GeoCoordinate(from, longitude).DistanceEstimate(new OsmSharp.Math.Geo.GeoCoordinate(to, longitude));
        //    return (int)(distance.Value / 3980000.0 * 256.0 * Math.Pow(2.0, _mapZoom - 2));
        //}

        //public int LongitudeOffsetToPixels(double from, double to, double latitude)
        //{
        //    OsmSharp.Units.Distance.Meter distance = new OsmSharp.Math.Geo.GeoCoordinate(latitude, from).DistanceEstimate(new OsmSharp.Math.Geo.GeoCoordinate(latitude, to));
        //    return (int)(distance.Value / 3980000.0 * 256.0 * Math.Pow(2.0, _mapZoom - 2));
        //}

        protected abstract MapBase CreateMap();

        //private void Map_MapChanged()
        //{
        //    MapPoint mapCenter = GetMapCenter();
        //    double mapZoom = GetMapZoom();
        //    if (Math.Abs(_mapCenter.Latitude - mapCenter.Latitude) + Math.Abs(_mapCenter.Longitude - mapCenter.Longitude) > Epsilon)
        //    {
        //        _mapCenter = mapCenter;
        //        OnMapCenterChanged();
        //    }
        //    if (Math.Abs(_mapZoom - mapZoom) > Epsilon)
        //    {
        //        _mapZoom = mapZoom;
        //        OnMapZoomChanged();
        //    }
        //    OnMapChanged();
        //}

        //protected virtual void OnMapChanged()
        //{
        //    EventHandler handler = MapChanged;
        //    if (handler != null)
        //        handler(this, EventArgs.Empty);
        //}

        //protected virtual void OnMapCenterChanged()
        //{
        //    EventHandler handler = MapCenterChanged;
        //    if (handler != null)
        //        handler(this, EventArgs.Empty);
        //}

        //protected virtual void OnMapZoomChanged()
        //{
        //    EventHandler handler = MapZoomChanged;
        //    if (handler != null)
        //        handler(this, EventArgs.Empty);
        //}
    }
}
