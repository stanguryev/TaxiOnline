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
using TaxiOnline.ClientInfrastructure.Data;

namespace TaxiOnline.ClientAdapters.Android.Services.Settings
{
    public class AndroidSettingsAdapter : SettingsAdapter, IAndroidSettingsService
    {
        private const string ServerEndPointName = "ServerEndPoint";
        private const string MapModeName = "MapMode";

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
            _settingsObject.MapMode = (MapMode)_preferences.GetInt(MapModeName, 0);
        }

        public override void SaveSettings()
        {
            using (ISharedPreferencesEditor preferencesEditor = _preferences.Edit())
            {
                preferencesEditor.PutString(ServerEndPointName, _settingsObject.ServerEndpointAddress);
                preferencesEditor.PutInt(MapModeName, (int)_settingsObject.MapMode);
                preferencesEditor.Commit();
            }
        }
    }
}