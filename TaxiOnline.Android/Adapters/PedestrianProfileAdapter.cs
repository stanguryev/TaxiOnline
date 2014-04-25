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
using TaxiOnline.Toolkit.Collections.Helpers;
using TaxiOnline.Toolkit.Events;
using TaxiOnline.Android.Helpers;
using TaxiOnline.Android.Activities;
using TaxiOnline.ClientInfrastructure.Data;

namespace TaxiOnline.Android.Adapters
{
    public class PedestrianProfileAdapter : BaseAdapter
    {
        private readonly Activity _context;
        private readonly PedestrianProfileModel _model;
        private List<DriverModel> _items;

        public PedestrianProfileAdapter(Activity context, PedestrianProfileModel model)
        {
            _context = context;
            _model = model;
            _model.DriversChanged += Model_DriversChanged;
            _model.DriversCollectionChanged += Model_DriversCollectionChanged;
            UpdateDrivers();
        }

        public override Java.Lang.Object GetItem(int position)
        {
            return position;
        }

        public override long GetItemId(int position)
        {
            return position;
        }

        public override int Count
        {
            get { return _items.Count + 1; }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            int layoutId = position == 0 ? Resource.Layout.PedestrianSelfInfoLayout : Resource.Layout.DriverInfoLayout;
            View view = _context.LayoutInflater.Inflate(layoutId, parent, false);
            if (position == 0)
                HookCurrentModelToView(view, parent);
            else
                HookModelToView(view, _items[position - 1], parent);
            return view;
        }

        private void HookCurrentModelToView(View view, ViewGroup upperView)
        {
            MapPoint mapCenter = _model.Map.MapService.Map.MapCenter;
            int x = upperView.Width / 2 - _model.Map.MapService.Map.LongitudeOffsetToPixels(mapCenter.Longitude, _model.CurrentLocation.Longitude, mapCenter.Latitude);
            int y = upperView.Height / 2 - _model.Map.MapService.Map.LatitudeOffsetToPixels(mapCenter.Latitude, _model.CurrentLocation.Latitude, mapCenter.Longitude);
            view.LayoutParameters = new AbsoluteLayout.LayoutParams(32, 32, x, y);
        }

        private void HookModelToView(View view, DriverModel driverModel, ViewGroup upperView)
        {
            MapPoint mapCenter = _model.Map.MapService.Map.MapCenter;
            int x = upperView.Width / 2 - _model.Map.MapService.Map.LongitudeOffsetToPixels(mapCenter.Longitude, driverModel.CurrentLocation.Longitude, mapCenter.Latitude);
            int y = upperView.Height / 2 - _model.Map.MapService.Map.LatitudeOffsetToPixels(mapCenter.Latitude, driverModel.CurrentLocation.Latitude, mapCenter.Longitude);
            view.LayoutParameters = new AbsoluteLayout.LayoutParams(32, 32, x, y);
            ImageView driverIconImageView = view.FindViewById<ImageView>(Resource.Id.driverIconImageView);
            driverIconImageView.Hover += (sender, e) =>
            {
                ShowDriverInfoToast(driverModel, driverIconImageView);
            };
            //throw new NotImplementedException();
        }

        private void UpdateDrivers()
        {
            IEnumerable<DriverModel> modelCollection = _model.Drivers;
            _items = modelCollection == null ? new List<DriverModel>() : modelCollection.ToList();
            NotifyDataSetChanged();
        }

        private void ShowDriverInfoToast(DriverModel driverModel, View briefView)
        {
            using (Toast driverInfoToast = new Toast(_context.Application.BaseContext)
                {
                    View = _context.LayoutInflater.Inflate(Resource.Layout.DriverPopupDetailsLayout, null),
                    Duration = ToastLength.Long
                })
            {
                driverInfoToast.SetGravity(GravityFlags.Center, 0, 0);
                HookModelToDetailsView(driverInfoToast.View, driverModel);
                driverInfoToast.Show();
            }
        }

        private void HookModelToDetailsView(View view, DriverModel driverModel)
        {
            _model.SelectedDriver = driverModel;
            view.Click += (sender, e) => UIHelper.GoResultActivity(_context, typeof(PedestrianProfileRequestActivity), 1);
            Button quickCallToDriverButton = view.FindViewById<Button>(Resource.Id.quickCallToDriverButton);
            quickCallToDriverButton.Click += (sender, e) =>
            {
                if (!_model.CallToDriver(driverModel).IsValid)
                    using (Toast errorToast = Toast.MakeText(Application.Context, Resource.String.PhoneCallError, ToastLength.Short))
                        errorToast.Show();
            };
            TextView driverPopupCarBrandTextView = view.FindViewById<TextView>(Resource.Id.driverPopupCarBrandTextView);
            TextView driverPopupCarColorTextView = view.FindViewById<TextView>(Resource.Id.driverPopupCarColorTextView);
            TextView driverPopupCarNumberTextView = view.FindViewById<TextView>(Resource.Id.driverPopupCarNumberTextView);
            TextView driverPopupPersonNameTextView = view.FindViewById<TextView>(Resource.Id.driverPopupPersonNameTextView);
            TextView driverPopupPhoneNumberTextView = view.FindViewById<TextView>(Resource.Id.driverPopupPhoneNumberTextView);
            TextView driverPopupSkypeNumberTextView = view.FindViewById<TextView>(Resource.Id.driverPopupSkypeNumberTextView);
            driverPopupPersonNameTextView.Text = driverModel.PersonName;
            driverPopupCarColorTextView.Text = driverModel.CarColor;
            driverPopupCarBrandTextView.Text = driverModel.CarBrand;
            driverPopupCarNumberTextView.Text = driverModel.CarNumber;
            driverPopupPhoneNumberTextView.Text = driverModel.PhoneNumber;
            driverPopupSkypeNumberTextView.Text = driverModel.SkypeNumber;
        }

        private void Model_DriversChanged(object sender, EventArgs e)
        {
            _context.RunOnUiThread(UpdateDrivers);
        }

        private void Model_DriversCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            _context.RunOnUiThread(() => ObservableCollectionHelper.ApplyChanges(e, _items));
        }
    }
}