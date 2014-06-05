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
using TaxiOnline.ClientServicesAdapter.Map;
using TaxiOnline.ClientInfrastructure.Data;
using TaxiOnline.MapEngine.Android.Views;

namespace TaxiOnline.ClientAdapters.Android.Services.Map
{
    public class AndroidMapAdapter : MapAdapter, IAndroidMapService
    {
        public AndroidMapAdapter(IAndroidSettingsService settings)
            : base(settings.CurrentSettings.MapMode)
        {

        }

        protected override MapWrapperBase CreateMapWrapper()
        {
            return new AndroidMapWrapper();
        }

        //public IDisposable VisualizeMap(Context context, ViewGroup viewGroup)
        //{
        //    return ((AndroidMapWrapper)_map).VisualizeMap(context, viewGroup);
        //}

        public void HookMap(View map)
        {
            ((AndroidMapWrapper)_map).HookMap((MapView)map);
        }
    }
}