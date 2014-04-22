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