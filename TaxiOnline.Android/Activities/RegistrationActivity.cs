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
using TaxiOnline.ClientInfrastructure.Data;

namespace TaxiOnline.Android.Activities
{
    [Activity]
    public class RegistrationActivity : Activity
    {
        private CityModel _model;

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            AuthenticationActivity authenticationActivity = UIHelper.GetUpperActivity<AuthenticationActivity>(this, bundle);
            if (authenticationActivity != null)
                _model = authenticationActivity.CityModel;
            SetContentView(Resource.Layout.RegistrationLayout);
            HookModel();
        }

        private void HookModel()
        {
            if (_model == null)
                return;
            Button confirmRegistrationButton = FindViewById<Button>(Resource.Id.confirmRegistrationButton);
            confirmRegistrationButton.Click += (sender, e) =>
            {
                RunAuthentication();
                SetResult(Result.Ok);
                Finish();
            };
            Button cancelRegistrationButton = FindViewById<Button>(Resource.Id.cancelRegistrationButton);
            cancelRegistrationButton.Click += (sender, e) =>
            {
                SetResult(Result.Canceled);
                Finish();
            };
        }

        private void RunAuthentication()
        {
            RadioButton pedestrianRadioButton = FindViewById<RadioButton>(Resource.Id.pedestrianRadioButton);
            RadioButton driverRadioButton = FindViewById<RadioButton>(Resource.Id.driverRadioButton);
            AuthenticationRequestModel request = null;// _model.CreateAuthenticationRequest(, null);
            if (pedestrianRadioButton.Checked)
            {
                request = _model.CreateAuthenticationRequest(ParticipantTypes.Pedestrian);
                FillPedestrianRequest((PedestrianAuthenticationRequestModel)request);
            }
            else if (driverRadioButton.Checked)
            {
                request = _model.CreateAuthenticationRequest(ParticipantTypes.Driver);
                FillDriverRequest((DriverAuthenticationRequestModel)request);
            }
            else
            {
                using (Toast errorToast = Toast.MakeText(Application.BaseContext, Resources.GetString(Resource.String.NoParticipationRole), ToastLength.Short))
                    errorToast.Show();
                return;
            }
            _model.BeginAuthenticate(request);
        }

        private void FillPedestrianRequest(PedestrianAuthenticationRequestModel pedestrianAuthenticationRequestModel)
        {
            EditText pedestrianSkypeNumberEditText = FindViewById<EditText>(Resource.Id.pedestrianSkypeNumberEditText);
            EditText pedestrianPhoneNumberEditText = FindViewById<EditText>(Resource.Id.pedestrianPhoneNumberEditText);
            pedestrianAuthenticationRequestModel.SkypeNumber = pedestrianSkypeNumberEditText.Text;
            pedestrianAuthenticationRequestModel.PhoneNumber = pedestrianPhoneNumberEditText.Text;
        }

        private void FillDriverRequest(DriverAuthenticationRequestModel driverAuthenticationRequestModel)
        {
            EditText driverSkypeNumberEditText = FindViewById<EditText>(Resource.Id.driverSkypeNumberEditText);
            EditText driverPhoneNumberEditText = FindViewById<EditText>(Resource.Id.driverPhoneNumberEditText);
            EditText carColorEditText = FindViewById<EditText>(Resource.Id.carColorEditText);
            driverAuthenticationRequestModel.SkypeNumber = driverSkypeNumberEditText.Text;
            driverAuthenticationRequestModel.PhoneNumber = driverPhoneNumberEditText.Text;
            driverAuthenticationRequestModel.CarColor = carColorEditText.Text;
        }
    }
}