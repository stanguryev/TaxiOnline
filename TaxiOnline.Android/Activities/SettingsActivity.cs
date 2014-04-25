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
    [Activity(Label = "@string/ApplicationName")]
    public class SettingsActivity : Activity
    {
        private Logic.Models.SettingsModel _model;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            MainActivity mainActiviy = UIHelper.GetUpperActivity<MainActivity>(this, bundle);
            if (mainActiviy != null)
                _model = mainActiviy.Model.Settings;
            SetContentView(Resource.Layout.SettingsLayout);
            HookModel();
        }

        private void HookModel()
        {
            if (_model == null)
                return;
            EditText serverAddressEditText = FindViewById<EditText>(Resource.Id.serverAddressEditText);
            serverAddressEditText.Text = _model.CurrentSettings.ServerEndpointAddress;
            Action writeSettings = () =>
            {
                _model.CurrentSettings.ServerEndpointAddress = serverAddressEditText.Text;
            };
            Button applySettingsButton = FindViewById<Button>(Resource.Id.applySettingsButton);
            applySettingsButton.Click += (sender, e) =>
            {
                writeSettings();
                _model.SaveSettings();
                Finish();
            };
            Button restartApplicationForSettingsButton = FindViewById<Button>(Resource.Id.restartApplicationForSettingsButton);
            restartApplicationForSettingsButton.Click += (sender, e) =>
            {
                writeSettings();
                _model.SaveSettings();
                Java.Lang.JavaSystem.Exit(0);
            };
            Button cancelSettingsButton = FindViewById<Button>(Resource.Id.cancelSettingsButton);
            cancelSettingsButton.Click += (sender, e) => Finish();
        }
    }
}