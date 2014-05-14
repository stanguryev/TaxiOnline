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
    public partial class AndroidHardwareAdapter : HardwareAdapter, IAndroidHardwareService
    {

        public override string GetDeviceId()
        {
            return null;
        }
    }
}