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
using TaxiOnline.ClientInfrastructure.Data;

namespace TaxiOnline.Android.Adapters
{
    public class PersonsAdapter : BaseAdapter
    {
        private readonly Activity _context;
        private readonly CityModel _model;
        private List<PersonModel> _items;

        public PersonsAdapter(Activity context, CityModel model)
        {
            _context = context;
            _model = model;
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
            int layoutId = _items[position] is DriverModel ? Resource.Layout.DriverReducedInfoLayout : Resource.Layout.PedestrianReducedInfoLayout;
            View view = _context.LayoutInflater.Inflate(layoutId, parent, false);
            HookModelToView(view, _items[position], parent);
            return view;
        }

        private void HookModelToView(View view, PersonModel personModel, ViewGroup upperView)
        {
            int width = upperView.Width;
            int height = upperView.Height;
            double pixelsScale = 1.0;
            MapPoint mapCenter = _model.Map.CurrentCenter;
            int x = (int)(width / 2.0 + pixelsScale * (personModel.CurrentLocation.Longitude - mapCenter.Longitude));
            int y = (int)(height / 2.0 + pixelsScale * (personModel.CurrentLocation.Latitude - mapCenter.Latitude));
            view.LayoutParameters = new AbsoluteLayout.LayoutParams(view.Width, view.Height, x, y);
        }
    }
}