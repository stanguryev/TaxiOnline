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
using TaxiOnline.Android.Helpers;

namespace TaxiOnline.Android.Activities
{
    [Activity(Label = "@string/ApplicationName")]
    public class DriverProfileActivity : Activity
    {
        private DriverProfileModel _model;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            AuthenticationActivity authenticationActivity = UIHelper.GetUpperActivity<AuthenticationActivity>(this, bundle);
            if (authenticationActivity != null)
                _model = authenticationActivity.ActiveDriverProfileModel;
            SetContentView(Resource.Layout.DriverProfileLayout);
            HookModel();
        }

        private void HookModel()
        {
            if (_model == null)
                return;

        }
    }
}