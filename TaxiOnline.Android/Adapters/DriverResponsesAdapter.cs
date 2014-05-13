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
            _model.AcceptedResponsesChanged += Model_AcceptedResponsesChanged;
            _model.AcceptedResponsesCollectionChanged += Model_AcceptedResponsesCollectionChanged;
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
            TextView driverResponseTextView = view.FindViewById<TextView>(Resource.Id.driverResponseTextView);
            driverResponseTextView.Text = driverResponseModel.GetBriefInfo();
            Button callToDriverAfterResponseButton = view.FindViewById<Button>(Resource.Id.callToDriverAfterResponseButton);
            callToDriverAfterResponseButton.Click += (sender, e) =>
            {
                if (!_model.CallToDriver(driverResponseModel.ResponseAuthor).IsValid)
                    using (Toast errorToast = Toast.MakeText(Application.Context, Resource.String.PhoneCallError, ToastLength.Short))
                        errorToast.Show();
            };
        }

        private void UpdateDriverResponses()
        {
            IEnumerable<DriverResponseModel> modelCollection = _model.AcceptedResponses;
            _items = modelCollection == null ? new List<DriverResponseModel>() : modelCollection.ToList();
            NotifyDataSetChanged();
        }

        private void Model_AcceptedResponsesChanged(object sender, EventArgs e)
        {
            _context.RunOnUiThread(UpdateDriverResponses);
        }

        private void Model_AcceptedResponsesCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            _context.RunOnUiThread(() => ObservableCollectionHelper.ApplyChanges<DriverResponseModel>(e, _items));
        }
    }
}