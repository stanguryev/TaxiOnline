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
using OsmSharp.Android.UI;
using TaxiOnline.ClientInfrastructure.Android.Services;
using TaxiOnline.Android.Views;
using TaxiOnline.Android.Adapters;
using TaxiOnline.Android.Decorators;
using TaxiOnline.ClientInfrastructure.Exceptions;

namespace TaxiOnline.Android.Activities
{
    [Activity(Label = "@string/ApplicationName")]
    public class AuthenticationActivity : Activity
    {
        private enum Dialogs
        {
            Registration = 1
        }

        private CityModel _cityModel;
        private InteractionModel _interactionModel;
        private DriverProfileModel _activeDriverProfileModel;
        private PedestrianProfileModel _activePedestrianProfileModel;
        private ProgressDialogDecorator _authorizationProgressDialogDecorator;
        private IDisposable _currentMap;

        public InteractionModel InteractionModel
        {
            get { return _interactionModel; }
        }

        public CityModel CityModel
        {
            get { return _cityModel; }
        }

        public DriverProfileModel ActiveDriverProfileModel
        {
            get { return _activeDriverProfileModel; }
        }

        public PedestrianProfileModel ActivePedestrianProfileModel
        {
            get { return _activePedestrianProfileModel; }
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            _authorizationProgressDialogDecorator = new ProgressDialogDecorator(this, Resources.GetString(Resource.String.AuthorizingTitle), Resources.GetString(Resource.String.AuthorizingMessage));
            MainActivity mainActiviy = UIHelper.GetUpperActivity<MainActivity>(this, bundle);
            if (mainActiviy != null)
            {
                _interactionModel = mainActiviy.Model;
                if (_interactionModel != null)
                    _cityModel = _interactionModel.CurrentCity;
            }
            SetContentView(Resource.Layout.AuthenticationLayout);
            //HookModel();
        }

        protected override void OnResume()
        {
            base.OnResume();
            HookModel();
        }

        protected override void OnPause()
        {
            base.OnPause();
            UnhookModel();
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == (int)Dialogs.Registration && resultCode == Result.Ok)
                _authorizationProgressDialogDecorator.Show();
        }

        private void HookModel()
        {
            if (_interactionModel == null || _cityModel == null)
                return;
            _cityModel.PersonsRequestFailed += CityModel_PersonsRequestFailed;
            _cityModel.AuthenticationFailed += CityModel_AuthenticationFailed;
            _interactionModel.CurrentProfileChanged += InteractionModel_CurrentProfileChanged;
            Button registerButton = FindViewById<Button>(Resource.Id.registerButton);
            registerButton.Click += (sender, e) => UIHelper.GoResultActivity(this, typeof(RegistrationActivity), (int)Dialogs.Registration);
            LinearLayout mapLayout = FindViewById<LinearLayout>(Resource.Id.mapLayout);
            if (_currentMap == null)
                _currentMap = ((IAndroidMapService)_cityModel.Map.MapService).VisualizeMap(this, mapLayout);
            CanvasView personsView = FindViewById<CanvasView>(Resource.Id.personsView);
            personsView.Adapter = new PersonsAdapter(this, _cityModel);
        }

        private void UnhookModel()
        {
            if (_currentMap != null)
                _currentMap.Dispose();
            CanvasView personsView = FindViewById<CanvasView>(Resource.Id.personsView);
            if (personsView.Adapter != null)
            {
                IAdapter personsViewAdapter = personsView.Adapter;
                personsView.Adapter = null;
                personsViewAdapter.Dispose();
            }
            if (_interactionModel == null || _cityModel == null)
                return;
            _cityModel.PersonsRequestFailed -= CityModel_PersonsRequestFailed;
            _cityModel.AuthenticationFailed -= CityModel_AuthenticationFailed;
        }

        private void ActivateProfile()
        {
            DriverProfileModel driverProfileModel = _interactionModel.CurrentProfile as DriverProfileModel;
            if (driverProfileModel != null)
                GoDriverActivity(driverProfileModel);
            PedestrianProfileModel pedestrianProfileModel = _interactionModel.CurrentProfile as PedestrianProfileModel;
            if (pedestrianProfileModel != null)
                GoPedestrianActivity(pedestrianProfileModel);
        }

        private void GoDriverActivity(DriverProfileModel driverProfileModel)
        {
            _activeDriverProfileModel = driverProfileModel;
            UIHelper.GoActivity(this, typeof(DriverProfileActivity));
        }

        private void GoPedestrianActivity(PedestrianProfileModel pedestrianProfileModel)
        {
            _activePedestrianProfileModel = pedestrianProfileModel;
            UIHelper.GoActivity(this, typeof(PedestrianProfileActivity));
        }

        private void CityModel_PersonsRequestFailed(object sender, Toolkit.Events.ActionResultEventArgs e)
        {
            using (Toast errorToast = Toast.MakeText(Application.BaseContext, Resources.GetString(Resource.String.FailedToLoadPersons), ToastLength.Short))
                errorToast.Show();
        }

        private void InteractionModel_CurrentProfileChanged(object sender, EventArgs e)
        {
            RunOnUiThread(() =>
            {
                ActivateProfile();
                _authorizationProgressDialogDecorator.Hide();
            });
        }

        private void CityModel_AuthenticationFailed(object sender, Toolkit.Events.ActionResultEventArgs e)
        {
            RunOnUiThread(() =>
            {
                _authorizationProgressDialogDecorator.Hide();
                string message = Resources.GetString(Resource.String.AuthenticationFailed);
                HardwareServiceException hardwareServiceException = e.Result.FailException as HardwareServiceException;
                if (hardwareServiceException != null)
                {
                    switch (hardwareServiceException.ErrorType)
                    {
                        case TaxiOnline.ClientInfrastructure.Exceptions.Enums.HardwareServiceErrors.NoLocationService:
                            message = Resources.GetString(Resource.String.NoLocationServiceAvailable);
                            break;
                        default:
                            break;
                    }
                }
                using (Toast errorToast = Toast.MakeText(Application.BaseContext, message, ToastLength.Short))
                    errorToast.Show();
            });
        }
    }
}