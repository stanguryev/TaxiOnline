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
using TaxiOnline.Android.Decorators;

namespace TaxiOnline.Android.Adapters
{
    public class PedestrianProfileAdapter : BaseAdapter
    {
        private readonly Activity _context;
        private readonly PedestrianProfileModel _model;
        private List<DriverModel> _items;
        private ViewsCacheDecorator<DriverModel> _viewCache;
        private Lazy<View> _selfItemView;

        public PedestrianProfileAdapter(Activity context, PedestrianProfileModel model)
        {
            _context = context;
            _model = model;
            _viewCache = new ViewsCacheDecorator<DriverModel>(() => _context.LayoutInflater.Inflate(Resource.Layout.DriverInfoLayout, null, false));
            _selfItemView = new Lazy<View>(() => _context.LayoutInflater.Inflate(Resource.Layout.PedestrianSelfInfoLayout, null, false));
            _model.DriversChanged += Model_DriversChanged;
            _model.DriversCollectionChanged += Model_DriversCollectionChanged;
            model.Map.MapService.Map.MapCenterChanged += Map_MapCenterChanged;
            model.Map.MapService.Map.MapZoomChanged += Map_MapZoomChanged;
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
            //int layoutId = position == 0 ? Resource.Layout.PedestrianSelfInfoLayout : Resource.Layout.DriverInfoLayout;
            //View view = _context.LayoutInflater.Inflate(layoutId, parent, false);
            View view = position == 0 ? _selfItemView.Value : _viewCache.GetView(_items[position - 1]);
            if (position == 0)
                HookCurrentModelToView(view, parent);
            else
                HookModelToView(view, _items[position - 1], parent);
            return view;
        }

        private void HookCurrentModelToView(View view, ViewGroup upperView)
        {
            view.LayoutParameters = MapHelper.GetLayoutParams(upperView, _model.Map.MapService.Map, _model.CurrentLocation);
        }

        private void HookModelToView(View view, DriverModel driverModel, ViewGroup upperView)
        {
            view.LayoutParameters = MapHelper.GetLayoutParams(upperView, _model.Map.MapService.Map, driverModel.CurrentLocation);
            ImageView driverIconImageView = view.FindViewById<ImageView>(Resource.Id.driverIconImageView);
            driverIconImageView.Hover += (sender, e) =>
            {
                ShowDriverInfoToast(driverModel, driverIconImageView);
            };
        }

        private void UpdateDrivers()
        {
            IEnumerable<DriverModel> modelCollection = _model.Drivers;
            _items = modelCollection == null ? new List<DriverModel>() : modelCollection.ToList();
            _viewCache.NotifyFillStarted();
            NotifyDataSetChanged();
            _viewCache.NotifyFillFinished();
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

        private void Map_MapCenterChanged(object sender, EventArgs e)
        {
            _viewCache.NotifyFillStarted();
            NotifyDataSetChanged();
            _viewCache.NotifyFillFinished();
        }

        private void Map_MapZoomChanged(object sender, EventArgs e)
        {
            _viewCache.NotifyFillStarted();
            NotifyDataSetChanged();
            _viewCache.NotifyFillFinished();
        }
    }
}