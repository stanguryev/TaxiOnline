using Android.Content;
using Android.Views;
using OsmSharp.Android.UI;
using OsmSharp.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxiOnline.ClientInfrastructure.Data;
using TaxiOnline.ClientServicesAdapter.Map;

namespace TaxiOnline.ClientAdapters.Android.Services.Map
{
    public class AndroidMapWrapper : MapWrapperBase
    {
        private class MapViewWrapper : IDisposable
        {
            private readonly MapViewSurface _surface;
            private readonly MapView _mapView;

            public MapView MapView
            {
                get { return _mapView; }
            }

            public MapViewWrapper(Context context)
            {
                _surface = new MapViewSurface(context);
                _mapView = new MapView(context, _surface);
            }

            public void Dispose()
            {
                _mapView.Dispose();
                _surface.Dispose();
            }
        }

        private MapViewWrapper _mapViewWrapper;

        public IDisposable VisualizeMap(Context context, ViewGroup viewGroup)
        {
            _mapViewWrapper = new MapViewWrapper(context);
            _mapViewWrapper.MapView.Map = _map;
            SetMapCenter(_mapCenter);
            SetMapZoom(_mapZoom);
            _mapViewWrapper.MapView.MapTilt = 0;
            _mapViewWrapper.MapView.MapAllowTilt = false;
            viewGroup.AddView(_mapViewWrapper.MapView);
            return _mapViewWrapper;
        }

        protected override void SetMapCenter(MapPoint value)
        {
            if (_mapViewWrapper != null)
                _mapViewWrapper.MapView.MapCenter = new OsmSharp.Math.Geo.GeoCoordinate(value.Latitude, value.Longitude);
        }

        protected override void SetMapZoom(double value)
        {
            if (_mapViewWrapper != null)
                _mapViewWrapper.MapView.MapZoom = (float)value;
        }

        protected override ClientInfrastructure.Data.MapPoint GetMapCenter()
        {
            return _mapViewWrapper == null ? new MapPoint() : new MapPoint(_mapViewWrapper.MapView.MapCenter.Latitude, _mapViewWrapper.MapView.MapCenter.Longitude);
        }

        protected override double GetMapZoom()
        {
            return _mapViewWrapper == null ? 1.0 : _mapViewWrapper.MapView.MapZoom;
        }
    }
}
