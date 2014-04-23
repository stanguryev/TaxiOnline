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
    public class PedestrianProfileActivity : Activity
    {
        private PedestrianProfileModel _model;

        public PedestrianProfileModel Model
        {
            get { return _model; }
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            AuthenticationActivity authenticationActivity = UIHelper.GetUpperActivity<AuthenticationActivity>(this, bundle);
            if (authenticationActivity != null)
                _model = authenticationActivity.ActivePedestrianProfileModel;
            SetContentView(Resource.Layout.PedestrianProfileLayout);
            HookModel();
        }

        private void HookModel()
        {
            if (_model == null)
                return;

        }
    }
}