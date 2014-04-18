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
using TaxiOnline.ClientInfrastructure.Services;

namespace TaxiOnline.ClientInfrastructure.Android.Services
{
    public interface IAndroidMapService : IMapService
    {
        //void HookMapView(IMapView mapView);
    }
}