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
using TaxiOnline.Android.Helpers;
using TaxiOnline.Logic.Models;

namespace TaxiOnline.Android.Activities
{
    [Activity(Label = "My Activity")]
    public class PedestrianPopupDetailsActivity : Activity
    {
        private PedestrianRequestModel _model;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            DriverProfileActivity driverProfileActivity = UIHelper.GetUpperActivity<DriverProfileActivity>(this, bundle);
            if (driverProfileActivity != null)
                _model = driverProfileActivity.Model.SelectedPedestrianRequest;
            Window.AddFlags(WindowManagerFlags.NotTouchModal);
            SetContentView(Resource.Layout.PedestrianPopupDetailsLayout);
            HookModel();
        }

        private void HookModel()
        {
            if (_model == null)
                return;

        }
    }
}