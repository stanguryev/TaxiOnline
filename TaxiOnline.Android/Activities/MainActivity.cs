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
using TaxiOnline.ClientInfrastructure.Data;

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
            AndroidAdaptersExtender extender = new AndroidAdaptersExtender(PreferenceManager.GetDefaultSharedPreferences(Application.Context));
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
                System.Threading.CancellationToken cancelLoad = _connectionProgressDialogDecorator.ShowWithCancel();
                _model.BeginLoadCities(cancelLoad);
            };
            ImageButton settingsButton = ActionBar.CustomView.FindViewById<ImageButton>(Resource.Id.settingsButton);
            settingsButton.Click += (sender, e) => UIHelper.GoResultActivity(this, typeof(SettingsActivity), 1);
            //ImageSwitcher connectionStateImageSwitcher = ActionBar.CustomView.FindViewById<ImageSwitcher>(Resource.Id.connectionStateImageSwitcher);
            //SetConnectionState(connectionStateImageSwitcher, _model.ConnectionState);
            //_model.ConnectionStateChanged += (sender, e) => RunOnUiThread(() => SetConnectionState(connectionStateImageSwitcher, _model.ConnectionState));
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
            System.Threading.CancellationToken cancelLoad = _connectionProgressDialogDecorator.ShowWithCancel();
            _model.BeginLoadCities(cancelLoad);
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
            ImageButton vkShareButton = FindViewById<ImageButton>(Resource.Id.vkShareButton);
            if (CanShareApplication("com.vkontakte.android"))
            {
                vkShareButton.SetImageDrawable(PackageManager.GetApplicationLogo("com.vkontakte.android"));
                vkShareButton.Click += (sender, e) => ShareApplication("com.vkontakte.android");
            }
            else
                vkShareButton.Visibility = ViewStates.Gone;
            //"com.facebook.katana"
            //"com.twitter.android"
            //"com.instagram.android"
            //"com.pinterest"
        }

        private void UnhookModel()
        {
            AutoCompleteTextView cityTextView = FindViewById<AutoCompleteTextView>(Resource.Id.cityTextView);
            if (cityTextView.Adapter != null)
                cityTextView.Adapter.Dispose();
        }

        private bool CanShareApplication(string packageName)
        {
            try
            {
                return PackageManager.GetPackageInfo(packageName, global::Android.Content.PM.PackageInfoFlags.MetaData) != null;
            }
            catch (global::Android.Content.PM.PackageManager.NameNotFoundException)
            {
                return false;
            }
        }

        private bool ShareApplication(string packageName)
        {
            using (AlertDialog.Builder dialogBuilder = new AlertDialog.Builder(this))
            {
                bool isCanceled = false;
                dialogBuilder.SetMessage(Resource.String.ConfirmShareApplication);
                dialogBuilder.SetNegativeButton(Resource.String.Cancel, (sender, e) => isCanceled = true);
                using (AlertDialog dialog = dialogBuilder.Create())
                    dialog.Show();
                if (isCanceled)
                    return false;
            }
            Intent socialNeworkIntent = PackageManager.GetLaunchIntentForPackage(packageName);
            if (socialNeworkIntent == null)
                return false;
            Intent shareIntent = new Intent();
            shareIntent.SetAction(Intent.ActionSend);
            shareIntent.SetPackage(packageName);
            shareIntent.PutExtra(Intent.ExtraTitle, Resources.GetString(Resource.String.ApplicationName));
            shareIntent.PutExtra(Intent.ExtraText, Resources.GetString(Resource.String.ApplicationDescription));
            StartActivity(shareIntent);
            return true;
        }

        private void SetConnectionState(ImageSwitcher connectionStateImageSwitcher, ConnectionState connectionState)
        {
            switch (connectionState)
            {
                case ConnectionState.Offline:
                    connectionStateImageSwitcher.SetImageResource(Resource.Drawable.OfflineIcon);
                    break;
                case ConnectionState.Connecting:
                    connectionStateImageSwitcher.SetImageResource(Resource.Drawable.ConnectingIcon);
                    break;
                case ConnectionState.Online:
                    connectionStateImageSwitcher.SetImageResource(Resource.Drawable.OnlineIcon);
                    break;
                default:
                    break;
            }
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

