using Android.Content;
using Android.Views;
using OsmSharp.Android.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxiOnline.ClientServicesAdapter.Map;

namespace TaxiOnline.ClientAdapters.Android.Services.Map
{
    public class AndroidMapWrapper : MapWrapperBase
    {
        private MapView _mapView;
        private Context _context;

        protected override void SetMapCenter(ClientInfrastructure.Data.MapPoint value)
        {
            if (_mapView != null && _context != null)
                _mapView.MapCenter = new OsmSharp.Math.Geo.GeoCoordinate(value.Latitude, value.Longitude);
        }

        protected override void SetMapZoom(double value)
        {
            if (_mapView != null && _context != null)
                _mapView.MapZoom = (float)value;
        }

        public void VisualizeMap(Context context, ViewGroup viewGroup)
        {
            _context = context;
            _mapView = new MapView(context, new MapViewSurface(context));
            SetMapCenter(_mapCenter);
            SetMapZoom(_mapZoom);
            _mapView.MapTilt = 0;
            _mapView.MapAllowTilt = false;
            viewGroup.AddView(_mapView);
        }
    }
}
