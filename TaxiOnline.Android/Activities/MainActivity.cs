using System;
using System.Linq;

using Android.App;
using Android.Content;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using TaxiOnline.Logic.Models;
using TaxiOnline.Android.Common;
using TaxiOnline.Android.Adapters;
using TaxiOnline.Android.Helpers;
using System.Collections.Generic;

namespace TaxiOnline.Android.Activities
{
    [Activity(Label = "@string/ApplicationName", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private readonly InteractionModel _model;

        public InteractionModel Model
        {
            get { return _model; }
        }

        public MainActivity()
        {
            _model = new InteractionModel(new AndroidAdaptersExtender());
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            SetContentView(Resource.Layout.MainLayout);//PreferenceManager
            HookModel();
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            UnhookModel();
        }

        private void HookModel()
        {
            _model.BeginLoadCities();
            AutoCompleteTextView cityTextView = FindViewById<AutoCompleteTextView>(Resource.Id.cityTextView);
            cityTextView.Adapter = new CitiesAdapter(this, _model);
            ImageButton chooseCityButton = FindViewById<ImageButton>(Resource.Id.chooseCityButton);
            chooseCityButton.Click += (sender, e) =>
            {
                IEnumerable<CityModel> cities = _model.Cities;
                CityModel city = cities == null ? null : cities.FirstOrDefault(c => c.Name == cityTextView.Text);
                if (city != null)
                {
                    _model.CurrentCity = city;
                    UIHelper.GoActivity(this, typeof(AuthenticationActivity));
                }
                else
                    using (Toast errorToast = Toast.MakeText(Application.BaseContext, Resource.String.NoCityFound, ToastLength.Short))
                        errorToast.Show();
            };
        }

        private void UnhookModel()
        {
            AutoCompleteTextView cityTextView = FindViewById<AutoCompleteTextView>(Resource.Id.cityTextView);
            if (cityTextView.Adapter != null)
                cityTextView.Adapter.Dispose();
        }
    }
}

