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
using TaxiOnline.ClientServicesAdapter;
using TaxiOnline.ClientInfrastructure.Services;
using TaxiOnline.ClientAdapters.Android.Services.Hardware;

namespace TaxiOnline.ClientAdapters.Android.Services
{
    public class AndroidServicesFactory : ServicesFactoryBase
    {
        public override IMapService GetCurrentMapService()
        {
            return null;
        }

        public override IHardwareService GetCurrentHardwareService()
        {
            return new AndroidHardwareAdapter();
        }
    }
}