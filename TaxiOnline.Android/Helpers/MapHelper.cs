using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using TaxiOnline.ClientInfrastructure.Data;
using TaxiOnline.ClientInfrastructure.ServicesEntities.Map;

namespace TaxiOnline.Android.Helpers
{
    public static class MapHelper
    {
        private const int IconSize = 32;

        public static ViewGroup.LayoutParams GetLayoutParams(ViewGroup mapView, IMap map, MapPoint location)
        {
            int x = mapView.Width / 2 + IconSize / 2 - map.LongitudeOffsetToPixels(map.MapCenter.Longitude, location.Longitude, map.MapCenter.Latitude);
            int y = mapView.Height / 2 + IconSize / 2 - map.LatitudeOffsetToPixels(map.MapCenter.Latitude, location.Latitude, map.MapCenter.Longitude);
            return new AbsoluteLayout.LayoutParams(IconSize, IconSize, x, y);
        }
    }
}