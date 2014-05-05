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
        //private PedestrianProfileRequestModel _selectedRequest;
        private int _notificationsCounter;
        private NotificationManager _notificationManager;
        private ProgressDialogDecorator _loadProgressDialogDecorator;
        private PedestrianProfileRequestModel[] _activeRequests;

        public PedestrianProfileModel Model
        {
            get { return _model; }
        }

        public PedestrianProfileRequestModel[] ActiveRequests
        {
            get { return _activeRequests; }
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            _loadProgressDialogDecorator = new ProgressDialogDecorator(this, Resources.GetString(Resource.String.LoadTitle), Resources.GetString(Resource.String.LoadDataMessage));
            AuthenticationActivity authenticationActivity = UIHelper.GetUpperActivity<AuthenticationActivity>(this, bundle);
            if (authenticationActivity != null)
                _model = authenticationActivity.ActivePedestrianProfileModel;
            SetContentView(Resource.Layout.PedestrianProfileLayout);
            _model.RequestsCollectionChanged += Model_RequestsCollectionChanged;
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

        private void HookRequest(PedestrianProfileRequestModel request)
        {
            request.AvailableResponsesCollectionChanged += CurrentRequest_AvailableResponsesCollectionChanged;
        }

        private void UnhookRequest(PedestrianProfileRequestModel request)
        {
            request.AvailableResponsesCollectionChanged -= CurrentRequest_AvailableResponsesCollectionChanged;
        }

        private void Model_RequestsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                foreach (PedestrianProfileRequestModel request in e.NewItems.OfType<PedestrianProfileRequestModel>().ToArray())
                    HookRequest(request);
            if (e.OldItems != null)
                foreach (PedestrianProfileRequestModel request in e.OldItems.OfType<PedestrianProfileRequestModel>().ToArray())
                    UnhookRequest(request);
        }

        private void CurrentRequest_AvailableResponsesCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
                RunOnUiThread(() =>
                {
                    _activeRequests = e.NewItems.OfType<DriverResponseModel>().Select(r => r.Request).Distinct().ToArray();
                    UIHelper.GetIntent(this, typeof(DriverResponsesActivity));
                    PendingIntent pendingIntent = PendingIntent.GetActivity(this, 0, null, PendingIntentFlags.UpdateCurrent);
                    Notification.Builder builder = new Notification.Builder(this);
                    builder.SetContentIntent(pendingIntent);
                    //builder.SetContentText();
                    _notificationManager.Notify(++_notificationsCounter, builder.Notification);
                });
        }
    }
}