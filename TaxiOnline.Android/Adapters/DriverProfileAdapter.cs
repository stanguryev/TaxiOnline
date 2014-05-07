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
using TaxiOnline.Android.Helpers;
using TaxiOnline.Android.Activities;
using TaxiOnline.Android.Decorators;

namespace TaxiOnline.Android.Adapters
{
    public class DriverProfileAdapter : BaseAdapter
    {
        private readonly Activity _context;
        private readonly DriverProfileModel _model;
        private List<PedestrianModel> _items;
        private ViewsCacheDecorator<PedestrianModel> _viewCache;
        private Lazy<View> _selfItemView;

        public DriverProfileAdapter(Activity context, DriverProfileModel model)
        {
            _context = context;
            _model = model;
            _viewCache = new ViewsCacheDecorator<PedestrianModel>(() => _context.LayoutInflater.Inflate(Resource.Layout.PedestrianInfoLayout, null, false));
            _selfItemView = new Lazy<View>(() => _context.LayoutInflater.Inflate(Resource.Layout.DriverSelfInfoLayout, null, false));
            _model.PedestriansChanged += Model_PedestriansChanged;
            _model.PedestriansCollectionChanged += Model_PedestriansCollectionChanged;
            _model.PedestrianRequestsCollectionChanged += Model_PedestrianRequestsCollectionChanged;
            _model.CurrentLocationChanged += Model_CurrentLocationChanged;
            model.Map.MapService.Map.MapCenterChanged += Map_MapCenterChanged;
            model.Map.MapService.Map.MapZoomChanged += Map_MapZoomChanged;
            UpdatePedestrians();
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

        private void HookModelToView(View view, PedestrianModel pedestrianModel, ViewGroup upperView)
        {
            view.LayoutParameters = MapHelper.GetLayoutParams(upperView, _model.Map.MapService.Map, pedestrianModel.CurrentLocation);
        }

        private void UpdatePedestrians()
        {
            IEnumerable<PedestrianModel> modelCollection = _model.Pedestrians;
            _items = modelCollection == null ? new List<PedestrianModel>() : modelCollection.ToList();
            NotifyDataSetChanged();
        }

        private void Model_PedestriansChanged(object sender, EventArgs e)
        {
            _context.RunOnUiThread(UpdatePedestrians);
        }

        private void Model_PedestriansCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            _context.RunOnUiThread(() => ObservableCollectionHelper.ApplyChanges(e, _items));
        }

        private void Model_PedestrianRequestsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            _context.RunOnUiThread(() =>
            {
                if (e.NewItems != null)
                    foreach (PedestrianRequestModel request in e.NewItems.OfType<PedestrianRequestModel>().ToArray())
                    {
                        _model.SelectedPedestrianRequest = request;
                        UIHelper.GoActivity(_context, typeof(PedestrianPopupDetailsActivity));
                    }
            });
        }

        private void Model_CurrentLocationChanged(object sender, EventArgs e)
        {
            _context.RunOnUiThread(() =>
            {
                _viewCache.NotifyFillStarted();
                NotifyDataSetChanged();
                _viewCache.NotifyFillFinished();
            });
        }

        private void Map_MapCenterChanged(object sender, EventArgs e)
        {
            _context.RunOnUiThread(() =>
            {
                _viewCache.NotifyFillStarted();
                NotifyDataSetChanged();
                _viewCache.NotifyFillFinished();
            });
        }

        private void Map_MapZoomChanged(object sender, EventArgs e)
        {
            _context.RunOnUiThread(() =>
            {
                _viewCache.NotifyFillStarted();
                NotifyDataSetChanged();
                _viewCache.NotifyFillFinished();
            });
        }
    }
}