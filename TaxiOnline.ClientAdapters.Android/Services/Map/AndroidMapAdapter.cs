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

namespace TaxiOnline.ClientAdapters.Android.Services.Map
{
    public class AndroidMapAdapter : MapAdapter, IAndroidMapService
    {
        public AndroidMapAdapter()
        {

        }

        protected override MapWrapperBase CreateMapWrapper()
        {
            return new AndroidMapWrapper();
        }

        public void VisualizeMap(Context context, ViewGroup viewGroup)
        {
            ((AndroidMapWrapper)_map).VisualizeMap(context, viewGroup);
        }
    }
}