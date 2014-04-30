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
using TaxiOnline.ClientInfrastructure.Android.Services;
using TaxiOnline.ClientAdapters.Android.Services.Settings;
using TaxiOnline.ClientAdapters.Android.Services.Map;

namespace TaxiOnline.ClientAdapters.Android.Services
{
    public class AndroidServicesFactory : ServicesFactoryBase
    {
        private readonly Lazy<IMapService> _mapService;
        private readonly Lazy<IAndroidSettingsService> _settingsService;
        private readonly Lazy<IAndroidHardwareService> _hardwareService;

        public AndroidServicesFactory(ISharedPreferences preferences)
        {
            _settingsService = new Lazy<IAndroidSettingsService>(() => new AndroidSettingsAdapter(preferences), true);
            _mapService = new Lazy<IMapService>(() => new AndroidMapAdapter(_settingsService.Value), true);
            _hardwareService = new Lazy<IAndroidHardwareService>(() => new AndroidHardwareAdapter(), true);
        }

        public override IMapService GetCurrentMapService()
        {
            return _mapService.Value;
        }

        public override IHardwareService GetCurrentHardwareService()
        {
            return _hardwareService.Value;
        }

        public override ISettingsService GetCurrentSettingsService()
        {
            return _settingsService.Value;
        }
    }
}