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
using TaxiOnline.Android.Adapters;
using TaxiOnline.Logic.Models;
using TaxiOnline.Android.Helpers;

namespace TaxiOnline.Android.Activities
{
    [Activity(Label = "@string/ApplicationName")]
    public class DriverResponsesActivity : Activity
    {
        private PedestrianProfileRequestModel _model;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            PedestrianProfileActivity pedestrianProfileActivity = UIHelper.GetUpperActivity<PedestrianProfileActivity>(this, bundle);
            if (pedestrianProfileActivity != null)
                _model = pedestrianProfileActivity.CurrentRequest;
            SetContentView(Resource.Layout.DriverResponsesLayout);
            HookModel();
        }

        private void HookModel()
        {
            if (_model == null)
                return;
            ListView driverResponsesListView = FindViewById<ListView>(Resource.Id.driverResponsesListView);
            driverResponsesListView.Adapter = new DriverResponsesAdapter(this, _model);
        }
    }
}