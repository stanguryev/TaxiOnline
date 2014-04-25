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
using TaxiOnline.Android.Views;
using TaxiOnline.Android.Adapters;
using TaxiOnline.ClientInfrastructure.Android.Services;
using TaxiOnline.Android.Decorators;

namespace TaxiOnline.Android.Activities
{
    [Activity(Label = "@string/ApplicationName")]
    public class PedestrianProfileActivity : Activity
    {
        private PedestrianProfileModel _model;
        private PedestrianProfileRequestModel _currentRequest;
        private NotificationManager _notificationManager;
        private ProgressDialogDecorator _loadProgressDialogDecorator;

        public PedestrianProfileModel Model
        {
            get { return _model; }
        }

        public PedestrianProfileRequestModel CurrentRequest
        {
            get { return _currentRequest; }
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            _loadProgressDialogDecorator = new ProgressDialogDecorator(this, Resources.GetString(Resource.String.LoadTitle), Resources.GetString(Resource.String.LoadDataMessage));
            AuthenticationActivity authenticationActivity = UIHelper.GetUpperActivity<AuthenticationActivity>(this, bundle);
            if (authenticationActivity != null)
                _model = authenticationActivity.ActivePedestrianProfileModel;
            SetContentView(Resource.Layout.PedestrianProfileLayout);
            _model.CurrentRequestChanged += Model_CurrentRequestChanged;
            _notificationManager = (NotificationManager)GetSystemService(Application.NotificationService);
            HookModel();
        }

        private void HookModel()
        {
            if (_model == null)
                return;
            _model.LoadCompleted += (sender, e) => _loadProgressDialogDecorator.Hide();
            _model.EnumrateDriversFailed += (sender, e) =>
            {
                using (Toast errorToast = Toast.MakeText(Application.BaseContext, Resources.GetString(Resource.String.FailedToEnumrateDrivers), ToastLength.Short))
                    errorToast.Show();
                _loadProgressDialogDecorator.Hide();
            };
            //_model.CheckCurrentRequestFailed += (sender, e) =>
            //{
            //    using (Toast errorToast = Toast.MakeText(Application.BaseContext, Resources.GetString(Resource.String.FailedToCheckCurrentRequest), ToastLength.Short))
            //        errorToast.Show();
            //    _loadProgressDialogDecorator.Hide();
            //};
            _loadProgressDialogDecorator.Show();
            _model.BeginLoad();
            LinearLayout mapLayout = FindViewById<LinearLayout>(Resource.Id.mapLayout);
            ((IAndroidMapService)_model.Map.MapService).VisualizeMap(this, mapLayout);
            CanvasView pedestrianProfileView = FindViewById<CanvasView>(Resource.Id.pedestrianProfileView);
            pedestrianProfileView.Adapter = new PedestrianProfileAdapter(this, _model);
        }

        private void HookCurrentRequest()
        {
            _currentRequest.AvailableResponsesCollectionChanged += CurrentRequest_AvailableResponsesCollectionChanged;
        }

        private void UnhookCurrentRequest()
        {
            _currentRequest.AvailableResponsesCollectionChanged -= CurrentRequest_AvailableResponsesCollectionChanged;
        }

        private void Model_CurrentRequestChanged(object sender, EventArgs e)
        {
            RunOnUiThread(() =>
            {
                if (_currentRequest != null)
                    UnhookCurrentRequest();
                _currentRequest = _model.CurrentRequest;
                if (_currentRequest != null)
                    HookCurrentRequest();
            });
        }

        private void CurrentRequest_AvailableResponsesCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RunOnUiThread(() =>
            {
                UIHelper.GetIntent(this, typeof(DriverResponsesActivity));
                PendingIntent pendingIntent = PendingIntent.GetActivity(this, 0, null, PendingIntentFlags.UpdateCurrent);
                Notification.Builder builder = new Notification.Builder(this);
                builder.SetContentIntent(pendingIntent);
                //builder.SetContentText();
                _notificationManager.Notify(1, builder.Notification);
            });
        }
    }
}