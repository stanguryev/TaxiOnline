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
            if (_interactionModel == null)
                return;
            HookCityModel();
            _interactionModel.CurrentProfileChanged += InteractionModel_CurrentProfileChanged;
            _interactionModel.CurrentCityChanged += InteractionModel_CurrentCityChanged;
            Button registerButton = FindViewById<Button>(Resource.Id.registerButton);
            registerButton.Click += RegisterButton_Click;
            LinearLayout mapLayout = FindViewById<LinearLayout>(Resource.Id.mapLayout);
            if (_currentMap == null)
                _currentMap = ((IAndroidMapService)_cityModel.Map.MapService).VisualizeMap(this, mapLayout);
            AutoCompleteTextView changeCityTextView = FindViewById<AutoCompleteTextView>(Resource.Id.changeCityTextView);
            changeCityTextView.Text = _cityModel.Name;
            changeCityTextView.Adapter = new CitiesAdapter(this, _interactionModel);
            Button changeCityButton = FindViewById<Button>(Resource.Id.changeCityButton);
            changeCityButton.Click += ChangeCityButton_Click;
        }

        private void UnhookModel()
        {
            if (_currentMap != null)
                _currentMap.Dispose();
            UnhookCityModel();
            AutoCompleteTextView changeCityTextView = FindViewById<AutoCompleteTextView>(Resource.Id.changeCityTextView);
            if (changeCityTextView.Adapter != null)
            {
                IAdapter changeCityAdapter = changeCityTextView.Adapter;
                changeCityTextView.Adapter = null;
                changeCityAdapter.Dispose();
            }
            if (_interactionModel != null)
            {
                _interactionModel.CurrentProfileChanged -= InteractionModel_CurrentProfileChanged;
                _interactionModel.CurrentCityChanged -= InteractionModel_CurrentCityChanged;
            }
            Button registerButton = FindViewById<Button>(Resource.Id.registerButton);
            registerButton.Click -= RegisterButton_Click;
        }

        private void HookCityModel()
        {
            if (_cityModel == null)
                return;
            _cityModel.PersonsRequestFailed += CityModel_PersonsRequestFailed;
            _cityModel.AuthenticationFailed += CityModel_AuthenticationFailed;
            CanvasView personsView = FindViewById<CanvasView>(Resource.Id.personsView);
            personsView.Adapter = new PersonsAdapter(this, _cityModel);
            AutoCompleteTextView changeCityTextView = FindViewById<AutoCompleteTextView>(Resource.Id.changeCityTextView);
            changeCityTextView.Text = _cityModel.Name;
        }

        private void UnhookCityModel()
        {
            CanvasView personsView = FindViewById<CanvasView>(Resource.Id.personsView);
            if (personsView.Adapter != null)
            {
                IAdapter personsViewAdapter = personsView.Adapter;
                personsView.Adapter = null;
                personsViewAdapter.Dispose();
            }
            if (_cityModel != null)
            {
                _cityModel.PersonsRequestFailed -= CityModel_PersonsRequestFailed;
                _cityModel.AuthenticationFailed -= CityModel_AuthenticationFailed;
            }
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

        private void RegisterButton_Click(object sender, EventArgs e)
        {
            UIHelper.GoResultActivity(this, typeof(RegistrationActivity), (int)Dialogs.Registration);
        }

        private void ChangeCityButton_Click(object sender, EventArgs e)
        {
            AutoCompleteTextView changeCityTextView = FindViewById<AutoCompleteTextView>(Resource.Id.changeCityTextView);
            IEnumerable<CityModel> cities = _interactionModel.Cities;
            CityModel city = cities == null ? null : cities.FirstOrDefault(c => c.Name == changeCityTextView.Text);
            if (city != null)
                _interactionModel.CurrentCity = city;
            else
                using (Toast errorToast = Toast.MakeText(Application.BaseContext, Resource.String.NoCityFound, ToastLength.Short))
                    errorToast.Show();
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

        private void InteractionModel_CurrentCityChanged(object sender, EventArgs e)
        {
            RunOnUiThread(() =>
            {
                UnhookCityModel();
                _cityModel = _interactionModel.CurrentCity;
                HookCityModel();
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