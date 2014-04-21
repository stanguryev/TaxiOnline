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
using TaxiOnline.ClientInfrastructure.Android.Services;
using TaxiOnline.ClientServicesAdapter.Hardware;
using TaxiOnline.ClientInfrastructure.Data;
using Android.Locations;

namespace TaxiOnline.ClientAdapters.Android.Services.Hardware
{
    public class AndroidHardwareAdapter : HardwareAdapter, IAndroidHardwareService
    {
        public override string GetDeviceId()
        {
            return null;
        }

        public override MapPoint GetCurrentLocation()
        {
            using (LocationManager locationManager = (LocationManager)Application.Context.GetSystemService(Application.LocationService))
            using (Location location = locationManager.GetLastKnownLocation(LocationManager.GpsProvider))
                return location == null ? new MapPoint() : new MapPoint(location.Latitude, location.Longitude);
        }
    }
}