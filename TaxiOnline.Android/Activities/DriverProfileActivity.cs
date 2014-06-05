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
using TaxiOnline.Android.Decorators;
using TaxiOnline.Android.Views;
using TaxiOnline.Android.Adapters;
using TaxiOnline.ClientInfrastructure.Android.Services;

namespace TaxiOnline.Android.Activities
{
    [Activity(Label = "@string/ApplicationName")]
    public class DriverProfileActivity : Activity
    {
        private DriverProfileModel _model;
        private ProgressDialogDecorator _loadProgressDialogDecorator;

        public DriverProfileModel Model
        {
            get { return _model; }
        }

        public DriverProfileActivity()
        {

        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            _loadProgressDialogDecorator = new ProgressDialogDecorator(this, Resources.GetString(Resource.String.LoadTitle), Resources.GetString(Resource.String.LoadDataMessage));
            AuthenticationActivity authenticationActivity = UIHelper.GetUpperActivity<AuthenticationActivity>(this, bundle);
            if (authenticationActivity != null)
                _model = authenticationActivity.ActiveDriverProfileModel;
            SetContentView(Resource.Layout.DriverProfileLayout);
            HookModel();
        }

        private void HookModel()
        {
            if (_model == null)
                return;
            _model.LoadCompleted += (sender, e) => _loadProgressDialogDecorator.Hide();
            _model.EnumratePedestriansFailed += (sender, e) =>
            {
                using (Toast errorToast = Toast.MakeText(Application.BaseContext, Resources.GetString(Resource.String.FailedToEnumratePedestrians), ToastLength.Short))
                    errorToast.Show();
                _loadProgressDialogDecorator.Hide();
            };
            //LinearLayout mapLayout = FindViewById<LinearLayout>(Resource.Id.mapLayout);
            //((IAndroidMapService)_model.Map.MapService).VisualizeMap(this, mapLayout);
            View map = FindViewById<View>(Resource.Id.map);
            ((IAndroidMapService)_model.Map.MapService).HookMap(map);
            CanvasView driverProfileView = FindViewById<CanvasView>(Resource.Id.driverProfileView);
            DriverProfileAdapter driverProfileAdapter = new DriverProfileAdapter(this, _model);
            driverProfileView.LayoutChange += (sender, e) => driverProfileAdapter.UpdatePedestrianInfoPopups();
            driverProfileView.Adapter = driverProfileAdapter;
            _loadProgressDialogDecorator.Show();
            _model.BeginLoad();
        }
    }
}