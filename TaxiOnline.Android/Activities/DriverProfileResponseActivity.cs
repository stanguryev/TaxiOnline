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
using TaxiOnline.Logic.Models;

namespace TaxiOnline.Android.Activities
{
    [Activity(Label = "@string/ApplicationName")]
    public class DriverProfileResponseActivity : Activity
    {
        private DriverProfileResponseModel _model;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);

            SetContentView(Resource.Layout.DriverProfileResponseLayout);
            HookModel();
        }

        private void HookModel()
        {
            if (_model == null)
                return;

        }
    }
}