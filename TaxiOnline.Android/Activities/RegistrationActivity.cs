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
    [Activity]
    public class RegistrationActivity : Activity
    {
        private InteractionModel _model;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            AuthenticationActivity authenticationActivity = UIHelper.GetUpperActivity<AuthenticationActivity>(this, bundle);
            if (authenticationActivity != null)
                _model = authenticationActivity.InteractionModel;
            SetContentView(Resource.Layout.RegistrationLayout);
            HookModel();
        }

        private void HookModel()
        {
            if (_model == null)
                return;
            Button confirmRegistrationButton = FindViewById<Button>(Resource.Id.confirmRegistrationButton);
            confirmRegistrationButton.Click += (sender, e) => SetResult(Result.Ok);
            Button cancelRegistrationButton = FindViewById<Button>(Resource.Id.cancelRegistrationButton);
            cancelRegistrationButton.Click += (sender, e) => SetResult(Result.Canceled);
        }
    }
}