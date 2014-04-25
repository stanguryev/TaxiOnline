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
using TaxiOnline.ClientServicesAdapter.Settings;
using TaxiOnline.ClientInfrastructure.Android.Services;
using TaxiOnline.ClientInfrastructure.ServicesEntities.Settings;

namespace TaxiOnline.ClientAdapters.Android.Services.Settings
{
    public class AndroidSettingsAdapter : SettingsAdapter, IAndroidSettingsService
    {
        private const string ServerEndPointName = "ServerEndPoint";

        private readonly ISharedPreferences _preferences;

        public ISharedPreferences Preferences
        {
            get { return _preferences; }
        }

        public AndroidSettingsAdapter(ISharedPreferences preferences)
        {
            _preferences = preferences;
        }

        public override void LoadSettings()
        {
            _settingsObject.ServerEndpointAddress = _preferences.GetString(ServerEndPointName, "http://10.188.112.31/TaxiOnline");
        }

        public override void SaveSettings()
        {
            _preferences.Edit().PutString(ServerEndPointName, _settingsObject.ServerEndpointAddress);
            _preferences.Edit().Commit();
        }
    }
}