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
using Android.Locations;
using TaxiOnline.ClientInfrastructure.Data;
using TaxiOnline.Toolkit.Events;
using System.Collections.Concurrent;

namespace TaxiOnline.ClientAdapters.Android.Services.Hardware
{
    public partial class AndroidHardwareAdapter
    {
        private readonly ConcurrentDictionary<Delegate, LocationListener> _locationChangedSubscriptions = new ConcurrentDictionary<Delegate, LocationListener>();

        private class LocationListener : Java.Lang.Object, ILocationListener
        {
            Action<Location> _locationChangedCallback;

            public LocationListener(Action<Location> locationChangedCallback)
            {
                _locationChangedCallback = locationChangedCallback;
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

        public override event EventHandler<ValueEventArgs<MapPoint>> LocationChanged
        {
            add { SubscribeToLocationChanged(value); }
            remove { UnsubscribeFromLocationChanged(value); }
        }

        public override ActionResult<MapPoint> GetCurrentLocation()
        {
            Criteria criteria = new Criteria();
            using (LocationManager locationManager = (LocationManager)Application.Context.GetSystemService(Application.LocationService))
            {
                string providerName = locationManager.GetBestProvider(criteria, true);
                using (System.Threading.EventWaitHandle waitLocation = new System.Threading.EventWaitHandle(false, System.Threading.EventResetMode.ManualReset))
                {
                    using (LocationListener locationListener = new LocationListener(l => waitLocation.Set()))
                        locationManager.RequestSingleUpdate(providerName, locationListener, null);
                    waitLocation.WaitOne(3000);
                    Location location = string.IsNullOrWhiteSpace(providerName) ? null : locationManager.GetLastKnownLocation(providerName);
                    if (location == null)
                        return ActionResult<MapPoint>.GetErrorResult(new NotSupportedException());
                    using (location)
                        return ActionResult<MapPoint>.GetValidResult(new MapPoint(location.Latitude, location.Longitude));
                }
            }
        }

        //public override void RequestLocation()
        //{
        //    Criteria criteria = new Criteria();
        //    using (LocationManager locationManager = (LocationManager)Application.Context.GetSystemService(Application.LocationService))
        //    {
        //        string providerName = locationManager.GetBestProvider(criteria, true);
        //        using (System.Threading.EventWaitHandle waitLocation = new System.Threading.EventWaitHandle(false, System.Threading.EventResetMode.ManualReset))
        //        {
        //            using (LocationListener locationListener = new LocationListener(l => { }))
        //                ;
        //        }
        //    }
        //}

        private void SubscribeToLocationChanged(EventHandler<ValueEventArgs<MapPoint>> handler)
        {
            //handler(this, new ValueEventArgs<MapPoint>(new MapPoint(59.9982349516688, 30.2508131012369)));
            //return;
            Criteria criteria = new Criteria();
            //using (LocationManager locationManager = (LocationManager)Application.Context.GetSystemService(Application.LocationService))
            LocationManager locationManager = (LocationManager)Application.Context.GetSystemService(Application.LocationService);
            {
                string providerName = locationManager.GetBestProvider(criteria, true);
                LocationListener locationListener = new LocationListener(location =>
                {
                    if (handler != null && location != null)
                        handler(this, new ValueEventArgs<MapPoint>(new MapPoint(location.Latitude, location.Longitude)));
                });
                _locationChangedSubscriptions.AddOrUpdate(handler, locationListener, (h, l) => locationListener);
                locationManager.RequestLocationUpdates(providerName, 100L, 1f, locationListener);
                locationManager.RequestSingleUpdate(providerName, locationListener, null);
            }
        }

        private void UnsubscribeFromLocationChanged(EventHandler<ValueEventArgs<MapPoint>> handler)
        {
            LocationListener locationListener;
            if (!_locationChangedSubscriptions.TryGetValue(handler, out locationListener) || locationListener == null)
                return;
            locationListener.Dispose();
        }
    }
}