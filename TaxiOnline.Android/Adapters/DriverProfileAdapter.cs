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

namespace TaxiOnline.Android.Adapters
{
    public class DriverProfileAdapter : BaseAdapter
    {
        private readonly Activity _context;
        private readonly DriverProfileModel _model;
        private List<PedestrianModel> _items;

        public DriverProfileAdapter(Activity context, DriverProfileModel model)
        {
            _context = context;
            _model = model;
            _model.PedestriansChanged += Model_PedestriansChanged;
            _model.PedestriansCollectionChanged += Model_PedestriansCollectionChanged;
            _model.PedestrianRequestsCollectionChanged += Model_PedestrianRequestsCollectionChanged;
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
            int layoutId = position == 0 ? Resource.Layout.DriverSelfInfoLayout : Resource.Layout.PedestrianInfoLayout;
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

        private void HookModelToView(View view, PedestrianModel pedestrianModel, ViewGroup parent)
        {
            //throw new NotImplementedException();
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
                        UIHelper.GoActivity(_context, typeof(PedestrianRequestModel));
                    }
            });
        }
    }
}