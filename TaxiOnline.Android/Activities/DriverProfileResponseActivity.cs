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
using TaxiOnline.Toolkit.Events;

namespace TaxiOnline.Android.Activities
{
    [Activity(Label = "@string/ApplicationName")]
    public class DriverProfileResponseActivity : Activity
    {
        private DriverProfileResponseModel _model;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            DriverProfileActivity driverProfileActivity = UIHelper.GetUpperActivity<DriverProfileActivity>(this, bundle);
            ActionResult<DriverProfileResponseModel> initResult = driverProfileActivity.Model.SelectedPedestrianRequest.InitResponse();
            if (driverProfileActivity != null && initResult.IsValid)
                _model = initResult.Result;
            SetContentView(Resource.Layout.DriverProfileResponseLayout);
            HookModel();
        }

        private void HookModel()
        {
            if (_model == null)
                return;
            _model.ConfirmApplied += Model_ConfirmApplied;
            _model.RejectApplied += Model_RejectApplied;
            Button confirmPedestrianRequestButton = FindViewById<Button>(Resource.Id.confirmPedestrianRequestButton);
            confirmPedestrianRequestButton.Click += (sender, e) => _model.Confirm();
            Button rejectPedestrianRequestButton = FindViewById<Button>(Resource.Id.rejectPedestrianRequestButton);
            rejectPedestrianRequestButton.Click += (sender, e) => _model.Reject();
        }

        private void Model_ConfirmApplied(object sender, ActionResultEventArgs e)
        {
            RunOnUiThread(Finish);
        }

        private void Model_RejectApplied(object sender, ActionResultEventArgs e)
        {
            RunOnUiThread(Finish);
        }
    }
}