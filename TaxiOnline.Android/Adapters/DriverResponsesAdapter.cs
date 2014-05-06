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
    public class DriverResponsesAdapter : BaseAdapter
    {
        private readonly Activity _context;
        private readonly PedestrianProfileModel _model;
        private List<DriverResponseModel> _items;

        public DriverResponsesAdapter(Activity context, PedestrianProfileModel model)
        {
            _context = context;
            _model = model;
            _model.RequestsChanged += Model_RequestsChanged;
            _model.RequestsCollectionChanged += Model_RequestsCollectionChanged;
            UpdateDriverResponses();
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
            get { return _items.Count; }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = _context.LayoutInflater.Inflate(Resource.Layout.DriverResponseLayout, parent, false);
            HookModelToView(view, _items[position]);
            return view;
        }

        private void HookModelToView(View view, DriverResponseModel driverResponseModel)
        {
            throw new NotImplementedException();
        }

        private void UpdateDriverResponses()
        {
            IEnumerable<PedestrianProfileRequestModel> modelCollection = _model.Requests;
            _items = modelCollection == null ? new List<DriverResponseModel>() : modelCollection.Select(col => col.Response).ToList();
            NotifyDataSetChanged();
        }

        private void Model_RequestsChanged(object sender, EventArgs e)
        {
            _context.RunOnUiThread(UpdateDriverResponses);
        }

        private void Model_RequestsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            _context.RunOnUiThread(() => ObservableCollectionHelper.ApplyChangesByObjects<PedestrianProfileRequestModel, DriverResponseModel>(e, _items, m => m.Response, m => m.Response));
        }
    }
}