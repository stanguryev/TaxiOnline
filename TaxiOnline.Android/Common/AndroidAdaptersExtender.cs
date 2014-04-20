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
using TaxiOnline.Logic.Common;
using TaxiOnline.ClientAdapters.Android.Services;
using Android.Preferences;

namespace TaxiOnline.Android.Common
{
    public class AndroidAdaptersExtender : AdaptersExtender
    {
        private const string ServerEndPointName = "ServerEndPoint";

        public AndroidAdaptersExtender()
            : base(new AndroidServicesFactory())
        {

        }

        public void ApplySettings(ISharedPreferences preferences)
        {
            string serverEndpointAddress = preferences.GetString(ServerEndPointName, "http://10.188.112.31/TaxiOnline");
            ServicesFactory.ConfigureDataService(serverEndpointAddress);
        }
    }
}