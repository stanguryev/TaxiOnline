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

namespace TaxiOnline.Android.Activities
{
    [Activity(Label = "@string/ApplicationName", NoHistory = true, FinishOnTaskLaunch = true)]
    public class DriverPopupDetailsActivity : Activity
    {
        private Logic.Models.DriverModel _model;
        private PedestrianProfileActivity _pedestrianProfileActivity;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            PedestrianProfileActivity pedestrianProfileActivity = UIHelper.GetUpperActivity<PedestrianProfileActivity>(this, bundle);
            _pedestrianProfileActivity = pedestrianProfileActivity;
            if (pedestrianProfileActivity != null)
                _model = pedestrianProfileActivity.Model.SelectedDriver;
            Window.AddFlags(WindowManagerFlags.NotTouchModal);
            SetContentView(Resource.Layout.DriverPopupDetailsLayout);
            HookModel();
        }

        private void HookModel()
        {
            if (_model == null)
                return;
            LinearLayout driverPopupLinearLayout = FindViewById<LinearLayout>(Resource.Id.driverPopupLinearLayout);
            driverPopupLinearLayout.Click += (sender, e) =>
            {
                UIHelper.GoResultActivity(_pedestrianProfileActivity, typeof(PedestrianProfileRequestActivity), 1);
                Finish();
            };
            Button quickCallToDriverButton = FindViewById<Button>(Resource.Id.quickCallToDriverButton);
            quickCallToDriverButton.Click += (sender, e) =>
            {
                Finish();
                if (!_pedestrianProfileActivity.Model.CallToDriver(_model).IsValid)
                    using (Toast errorToast = Toast.MakeText(Application.Context, Resource.String.PhoneCallError, ToastLength.Short))
                        errorToast.Show();
            };
            TextView driverPopupCarBrandTextView = FindViewById<TextView>(Resource.Id.driverPopupCarBrandTextView);
            TextView driverPopupCarColorTextView = FindViewById<TextView>(Resource.Id.driverPopupCarColorTextView);
            TextView driverPopupCarNumberTextView = FindViewById<TextView>(Resource.Id.driverPopupCarNumberTextView);
            TextView driverPopupPersonNameTextView = FindViewById<TextView>(Resource.Id.driverPopupPersonNameTextView);
            TextView driverPopupPhoneNumberTextView = FindViewById<TextView>(Resource.Id.driverPopupPhoneNumberTextView);
            TextView driverPopupSkypeNumberTextView = FindViewById<TextView>(Resource.Id.driverPopupSkypeNumberTextView);
            driverPopupPersonNameTextView.Text = _model.PersonName;
            driverPopupCarColorTextView.Text = _model.CarColor;
            driverPopupCarBrandTextView.Text = _model.CarBrand;
            driverPopupCarNumberTextView.Text = _model.CarNumber;
            driverPopupPhoneNumberTextView.Text = _model.PhoneNumber;
            driverPopupSkypeNumberTextView.Text = _model.SkypeNumber;
        }
    }
}