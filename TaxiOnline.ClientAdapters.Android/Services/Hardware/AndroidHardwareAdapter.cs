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
using Android.Telephony;
using TaxiOnline.Toolkit.Events;
using System.Threading;

namespace TaxiOnline.ClientAdapters.Android.Services.Hardware
{
    public class AndroidHardwareAdapter : HardwareAdapter, IAndroidHardwareService
    {
        private class LocationListener : Java.Lang.Object, ILocationListener
        {
            Action<Location> _locationChangedCallback;

            public LocationListener(Action<Location> locationChangedCallback)
            {
                locationChangedCallback = _locationChangedCallback;
            }

            public void OnLocationChanged(Location location)
            {
                _locationChangedCallback(location);
            }

            public void OnProviderDisabled(string provider)
            {
                
            }

            public void OnProviderEnabled(string provider)
            {
                
            }

            public void OnStatusChanged(string provider, Availability status, Bundle extras)
            {
                
            }
        }

        public override string GetDeviceId()
        {
            return null;
        }

        public override ActionResult<MapPoint> GetCurrentLocation()
        {
            Criteria criteria = new Criteria();
            using (LocationManager locationManager = (LocationManager)Application.Context.GetSystemService(Application.LocationService))
            {
                string providerName = locationManager.GetBestProvider(criteria, true);
                using (LocationListener locationListener = new LocationListener(l => { }))
                    locationManager.RequestSingleUpdate(providerName, locationListener, null);
                Location location = string.IsNullOrWhiteSpace(providerName) ? null : locationManager.GetLastKnownLocation(providerName);
                if (location == null)
                    return ActionResult<MapPoint>.GetErrorResult(new NotSupportedException());
                using (location)
                    return ActionResult<MapPoint>.GetValidResult(new MapPoint(location.Latitude, location.Longitude));
            }
        }

        public override ActionResult PhoneCall(string number)
        {
            //using(  TelephonyManager  telephonyManager=(TelephonyManager)Application.Context.GetSystemService(Application.TelephonyService))
            //  telephonyManager.
            string unifiedNumber = string.Format("tel:{0}", new string(number.Where(c => char.IsDigit(c) || c == '+').ToArray()));
            Intent callIntent = new Intent(Intent.ActionCall);
            callIntent.SetData(global::Android.Net.Uri.Parse(unifiedNumber));
            Application.Context.StartActivity(callIntent);
            return ActionResult.ValidResult;
        }
    }
}