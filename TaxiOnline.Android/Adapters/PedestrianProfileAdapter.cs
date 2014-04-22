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

namespace TaxiOnline.Android.Adapters
{
    public class PedestrianProfileAdapter : BaseAdapter
    {
        private readonly Activity _context;
        private readonly PedestrianProfileModel _model;
        private List<DriverModel> _items;
        private Toast _driverInfoToast;

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

        private void HookCurrentModelToView(View view, ViewGroup parent)
        {
            //throw new NotImplementedException();
        }

        private void HookModelToView(View view, DriverModel driverModel, ViewGroup parent)
        {
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
            if (_driverInfoToast == null)
            {
                _driverInfoToast = new Toast(_context.Application.BaseContext)
                {
                    View = _context.LayoutInflater.Inflate(Resource.Layout.DriverPopupDetailsLayout, null),
                    Duration = ToastLength.Long
                };
                _driverInfoToast.SetGravity(GravityFlags.Center, 0, 0);
                HookModelToDetailsView(_driverInfoToast.View, driverModel);
                _driverInfoToast.Show();
                _driverInfoToast.Dispose();
                _driverInfoToast = null;
            }
        }

        private void HookModelToDetailsView(View view, DriverModel driverModel)
        {
            Button callToDriverButton = view.FindViewById<Button>(Resource.Id.callToDriverButton);
            callToDriverButton.Click += (sender, e) =>
            {
                if (!_model.CallToDriver(driverModel).IsValid)
                    using (Toast errorToast = Toast.MakeText(Application.Context, Resource.String.PhoneCallError, ToastLength.Short))
                        errorToast.Show();
            };
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