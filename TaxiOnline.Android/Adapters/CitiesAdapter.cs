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

namespace TaxiOnline.Android.Adapters
{
    public class CitiesAdapter : BaseAdapter, IFilterable
    {
        private readonly Activity _context;
        private readonly InteractionModel _model;
        private List<CityModel> _items;
        private List<CityModel> _filteredItems;
        private readonly Lazy<CitySuggestionsFilter> _fitler;

        public List<CityModel> Items
        {
            get { return _items; }
        }

        public CitiesAdapter(Activity context, InteractionModel model)
        {
            _context = context;
            _model = model;
            _fitler = new Lazy<CitySuggestionsFilter>(() => new CitySuggestionsFilter(this), true);
            model.CitiesChanged += Model_CitiesChanged;
            UpdateCities();
        }

        public Filter Filter
        {
            get { return _fitler.Value; }
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
            get { return _filteredItems.Count; }
        }

        public override View GetView(int position, View convertView, ViewGroup parent)
        {
            View view = _context.LayoutInflater.Inflate(Resource.Layout.CityInfoLayout, parent, false);
            HookModelToView(view, _filteredItems[position]);
            return view;
        }

        public void SetFilter(IEnumerable<string> values)
        {
            _filteredItems = _items.Where(i => values.Contains(i.Name)).ToList();
            NotifyDataSetChanged();
        }

        private void HookModelToView(View view, CityModel cityModel)
        {
            TextView cityNameTextView = view.FindViewById<TextView>(Resource.Id.cityNameTextView);
            cityNameTextView.Text = cityModel.Name;
        }

        private void UpdateCities()
        {
            IEnumerable<CityModel> modelCollection = _model.Cities;
            _items = modelCollection == null ? new List<CityModel>() : modelCollection.ToList();
            NotifyDataSetChanged();
        }

        private void Model_CitiesChanged(object sender, EventArgs e)
        {
            _context.RunOnUiThread(UpdateCities);
        }
    }
}