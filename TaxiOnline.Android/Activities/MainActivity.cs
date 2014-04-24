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
using Android.Preferences;
using TaxiOnline.Android.Decorators;

namespace TaxiOnline.Android.Activities
{
    [Activity(Label = "@string/ApplicationName", MainLauncher = true, Icon = "@drawable/icon")]
    public class MainActivity : Activity
    {
        private readonly InteractionModel _model;
        private ProgressDialogDecorator _connectionProgressDialogDecorator;

        public InteractionModel Model
        {
            get { return _model; }
        }

        public MainActivity()
        {
            AndroidAdaptersExtender extender = new AndroidAdaptersExtender();
            extender.ApplySettings(PreferenceManager.GetDefaultSharedPreferences(Application.Context));
            _model = new InteractionModel(extender);
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            _connectionProgressDialogDecorator = new ProgressDialogDecorator(this, Resources.GetString(Resource.String.ConnectingToServerTitle), Resources.GetString(Resource.String.ConnectingToServerMessage));
            RequestWindowFeature(WindowFeatures.ActionBar);
            SetContentView(Resource.Layout.MainLayout);
            HookModel();
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            ActionBar.SetDisplayShowCustomEnabled(true);
            ActionBar.SetCustomView(Resource.Layout.ActionBarLayout);
            ActionBar.Show();
            ImageButton refreshCitiesButton = ActionBar.CustomView.FindViewById<ImageButton>(Resource.Id.refreshCitiesButton);
            refreshCitiesButton.Click += (sender, e) =>
            {
                _connectionProgressDialogDecorator.Show();
                _model.BeginLoadCities();
            };            
            ImageButton settingsButton = FindViewById<ImageButton>(Resource.Id.settingsButton);
            settingsButton.Click += (sender, e) => UIHelper.GoResultActivity(this, typeof(SettingsActivity), 1);
            return base.OnCreateOptionsMenu(menu);
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            UnhookModel();
        }

        private void HookModel()
        {
            _model.CitiesChanged += Model_CitiesChanged;
            _model.EnumrateCitiesFailed += Model_EnumrateCitiesFailed;
            _connectionProgressDialogDecorator.Show();
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

        private void Model_EnumrateCitiesFailed(object sender, Toolkit.Events.ActionResultEventArgs e)
        {
            RunOnUiThread(() =>
            {
                _connectionProgressDialogDecorator.Hide();
                using (Toast errorToast = Toast.MakeText(Application.BaseContext, Resources.GetString(Resource.String.ConnectToServerFailed), ToastLength.Short))
                    errorToast.Show();
            });
        }

        private void Model_CitiesChanged(object sender, EventArgs e)
        {
            RunOnUiThread(() => _connectionProgressDialogDecorator.Hide());
        }
    }
}

