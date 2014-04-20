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

namespace TaxiOnline.Android.Activities
{
    [Activity(Label = "@string/ApplicationName")]
    public class AuthenticationActivity : Activity
    {
        private CityModel _cityModel;
        private InteractionModel _interactionModel;
        private DriverProfileModel _activeDriverProfileModel;
        private PedestrianProfileModel _activePedestrianProfileModel;

        public InteractionModel InteractionModel
        {
            get { return _interactionModel; }
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
            MainActivity mainActiviy = UIHelper.GetUpperActivity<MainActivity>(this, bundle);
            if (mainActiviy != null)
            {
                _interactionModel = mainActiviy.Model;
                if (_interactionModel != null)
                    _cityModel = _interactionModel.CurrentCity;
            }
            SetContentView(Resource.Layout.AuthenticationLayout);
            HookModel();
        }

        protected override void OnActivityResult(int requestCode, Result resultCode, Intent data)
        {
            base.OnActivityResult(requestCode, resultCode, data);
            if (requestCode == 1 && resultCode == Result.Ok)
                ;
        }

        private void HookModel()
        {
            if (_interactionModel == null || _cityModel == null)
                return;
            _interactionModel.CurrentProfileChanged += InteractionModel_CurrentProfileChanged;
            Button registerButton = FindViewById<Button>(Resource.Id.registerButton);
            registerButton.Click += (sender, e) => UIHelper.GoResultActivity(this, typeof(RegistrationActivity), 1);
            HookMapService((IAndroidMapService)_cityModel.Map.MapService);
            CanvasView personsView = FindViewById<CanvasView>(Resource.Id.personsView);
            personsView.Adapter = new PersonsAdapter(this, _cityModel);
        }

        public void HookMapService(IAndroidMapService mapService)
        {
            MapView map = new MapView(this, new MapViewSurface(this));//FindViewById<MapView>(Resource.Id.map);
            LinearLayout mapLayout = FindViewById<LinearLayout>(Resource.Id.mapLayout);
            map.Map = new OsmSharp.UI.Map.Map();
            map.MapCenter = new OsmSharp.Math.Geo.GeoCoordinate(_cityModel.InitialCenter.Latitude, _cityModel.InitialCenter.Longitude);
            map.MapZoom = (float)_cityModel.InitialZoom;
            mapLayout.AddView(map);
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

        private void InteractionModel_CurrentProfileChanged(object sender, EventArgs e)
        {
            DriverProfileModel driverProfileModel = _interactionModel.CurrentProfile as DriverProfileModel;
            if (driverProfileModel != null)
                GoDriverActivity(driverProfileModel);
            PedestrianProfileModel pedestrianProfileModel = _interactionModel.CurrentProfile as PedestrianProfileModel;
            if (pedestrianProfileModel != null)
                GoPedestrianActivity(pedestrianProfileModel);
        }
    }
}