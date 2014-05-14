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
        private int _notificationsCounter;
        private NotificationManager _notificationManager;
        private ProgressDialogDecorator _loadProgressDialogDecorator;
        private Dictionary<DriverResponseModel, int> _responseIds = new Dictionary<DriverResponseModel, int>();

        public PedestrianProfileModel Model
        {
            get { return _model; }
        }

        protected override void OnCreate(Bundle bundle)
        {
            base.OnCreate(bundle);
            _loadProgressDialogDecorator = new ProgressDialogDecorator(this, Resources.GetString(Resource.String.LoadTitle), Resources.GetString(Resource.String.LoadDataMessage));
            AuthenticationActivity authenticationActivity = UIHelper.GetUpperActivity<AuthenticationActivity>(this, bundle);
            if (authenticationActivity != null)
                _model = authenticationActivity.ActivePedestrianProfileModel;
            SetContentView(Resource.Layout.PedestrianProfileLayout);
            _notificationManager = (NotificationManager)GetSystemService(Application.NotificationService);
            HookModel();
        }

        private void HookModel()
        {
            if (_model == null)
                return;
            _model.RequestsCollectionChanged += Model_RequestsCollectionChanged;
            _model.AcceptedResponsesChanged += Model_AcceptedResponsesChanged;
            _model.AcceptedResponsesCollectionChanged += Model_AcceptedResponsesCollectionChanged;
            _model.LoadCompleted += (sender, e) => _loadProgressDialogDecorator.Hide();
            _model.EnumrateDriversFailed += (sender, e) =>
            {
                using (Toast errorToast = Toast.MakeText(Application.BaseContext, Resources.GetString(Resource.String.FailedToEnumrateDrivers), ToastLength.Short))
                    errorToast.Show();
                _loadProgressDialogDecorator.Hide();
            };
            _model.EnumrateRequestsFailed += (sender, e) =>
            {
                using (Toast errorToast = Toast.MakeText(Application.BaseContext, Resource.String.FailedToEnumrateRequests, ToastLength.Short))
                    errorToast.Show();
                _loadProgressDialogDecorator.Hide();
            };
            _model.EnumrateAcceptedResponsesFailed += (sender, e) =>
            {
                using (Toast errorToast = Toast.MakeText(Application.BaseContext, Resource.String.FailedToEnumrateAcceptedResponses, ToastLength.Short))
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
            mapLayout.Clickable = true;
            mapLayout.Click += (sender, e) => _model.CheckedDriver = null;
        }        

        private void UpdateAcceptedResponses()
        {
            foreach (DriverResponseModel response in _responseIds.Keys.ToArray())
                RemoveResponseNotification(response);
            IEnumerable<DriverResponseModel> acceptedResponses = _model.AcceptedResponses;
            if (acceptedResponses != null)
                foreach (DriverResponseModel response in acceptedResponses)
                    AddResponseNotification(response);
        }

        private void AddResponseNotification(DriverResponseModel response)
        {
            if (_responseIds.ContainsKey(response))
                return;
            PendingIntent pendingIntent = PendingIntent.GetActivity(this, 0, UIHelper.GetIntent(this, typeof(DriverResponsesActivity)), PendingIntentFlags.UpdateCurrent);
            Notification.Builder builder = new Notification.Builder(this);
            builder.SetContentIntent(pendingIntent);
            builder.SetContentTitle(response.GetBriefInfo());
            builder.SetContentText(response.GetBriefInfo());
            builder.SetAutoCancel(true);
            builder.SetSmallIcon(Resource.Drawable.DriverIcon);
            _notificationManager.Notify(++_notificationsCounter, builder.Notification);
            _responseIds.Add(response, _notificationsCounter);
        }

        private void RemoveResponseNotification(DriverResponseModel response)
        {
            if (!_responseIds.ContainsKey(response))
                return;
            _notificationManager.Cancel(_responseIds[response]);
        }

        private void HookRequest(PedestrianProfileRequestModel request)
        {
            //request.Response.StateChanged += Response_StateChanged;
        }

        private void UnhookRequest(PedestrianProfileRequestModel request)
        {
            //request.Response.StateChanged -= Response_StateChanged;
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

        private void Model_AcceptedResponsesChanged(object sender, EventArgs e)
        {
            RunOnUiThread(UpdateAcceptedResponses);
        }

        private void Model_AcceptedResponsesCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RunOnUiThread(() =>
            {
                if (e.OldItems != null)
                    foreach (DriverResponseModel response in e.NewItems.OfType<DriverResponseModel>().ToArray())
                        RemoveResponseNotification(response);
                if (e.NewItems != null)
                    foreach (DriverResponseModel response in e.NewItems.OfType<DriverResponseModel>().ToArray())
                        AddResponseNotification(response);
            });
        }

        //private void Response_StateChanged(object sender, EventArgs e)
        //{
        //    RunOnUiThread(() =>
        //    {
        //        PendingIntent pendingIntent = PendingIntent.GetActivity(this, 0, UIHelper.GetIntent(this, typeof(DriverResponsesActivity)), PendingIntentFlags.UpdateCurrent);
        //        Notification.Builder builder = new Notification.Builder(this);
        //        builder.SetContentIntent(pendingIntent);
        //        //builder.SetContentText();
        //        _notificationManager.Notify(++_notificationsCounter, builder.Notification);
        //    });
        //}
    }
}