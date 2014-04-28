using Android.Content;
using Android.Views;
using OsmSharp.Android.UI;
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
        private MapView _mapView;
        private Context _context;

        public void VisualizeMap(Context context, ViewGroup viewGroup)
        {
            _context = context;
            _mapView = new MapView(context, new MapViewSurface(context));
            _mapView.Map = _map;
            SetMapCenter(_mapCenter);
            SetMapZoom(_mapZoom);
            _mapView.MapTilt = 0;
            _mapView.MapAllowTilt = false;
            viewGroup.AddView(_mapView);
        }

        protected override void SetMapCenter(MapPoint value)
        {
            if (_mapView != null)
                _mapView.MapCenter = new OsmSharp.Math.Geo.GeoCoordinate(value.Latitude, value.Longitude);
        }

        protected override void SetMapZoom(double value)
        {
            if (_mapView != null)
                _mapView.MapZoom = (float)value;
        }

        protected override ClientInfrastructure.Data.MapPoint GetMapCenter()
        {
            return _mapView == null ? new MapPoint() : new MapPoint(_mapView.MapCenter.Latitude, _mapView.MapCenter.Longitude);
        }

        protected override double GetMapZoom()
        {
            return _mapView == null ? 1.0 : _mapView.MapZoom;
        }
    }
}
