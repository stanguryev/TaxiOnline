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
using TaxiOnline.Toolkit.Events;
using TaxiOnline.Android.Helpers;

namespace TaxiOnline.Android.Activities
{
    [Activity(Label = "@string/ApplicationName")]
    public class PedestrianProfileRequestActivity : Activity
    {
        private PedestrianProfileRequestModel _model;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            PedestrianProfileActivity pedestrianProfileActivity = UIHelper.GetUpperActivity<PedestrianProfileActivity>(this, bundle);
            ActionResult<PedestrianProfileRequestModel> initResult = pedestrianProfileActivity.Model.InitRequest(pedestrianProfileActivity.Model.SelectedDriver);
            if (!initResult.IsValid)
            {
                using (Toast errorToast = Toast.MakeText(Application.BaseContext, Resource.String.FailedToInitRequest, ToastLength.Short))
                    errorToast.Show();
                Finish();
            }
            if (pedestrianProfileActivity != null)
                _model = initResult.Result;
            SetContentView(Resource.Layout.PedestrianProfileRequestLayout);
            HookModel();
        }

        private void HookModel()
        {
            if (_model == null)
                return;
            Button quickCallToDriverButton = FindViewById<Button>(Resource.Id.callToDriverButton);
            quickCallToDriverButton.Click += (sender, e) =>
            {
                if (!_model.CallToDriver().IsValid)
                    using (Toast errorToast = Toast.MakeText(Application.Context, Resource.String.PhoneCallError, ToastLength.Short))
                        errorToast.Show();
            };
            TextView messageToDriverEditText = FindViewById<TextView>(Resource.Id.messageToDriverEditText);
            messageToDriverEditText.TextChanged += (sender, e) => _model.Comment = messageToDriverEditText.Text;
            Button sendMessageToDriverButton = FindViewById<Button>(Resource.Id.sendMessageToDriverButton);
            sendMessageToDriverButton.Click += (sender, e) => _model.Confirm();
            Button cancelMessageToDriverButton = FindViewById<Button>(Resource.Id.cancelMessageToDriverButton);
            cancelMessageToDriverButton.Click += (sender, e) => _model.Cancel();

        }
    }
}