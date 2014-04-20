using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxiOnline.Logic.Models;

namespace TaxiOnline.Android.Adapters
{
    public class CitySuggestionsFilter : Filter
    {
        private readonly CitiesAdapter _adapter;

        public CitySuggestionsFilter(CitiesAdapter adapter)
        {
            _adapter = adapter;
        }

        protected override Filter.FilterResults PerformFiltering(Java.Lang.ICharSequence constraint)
        {
            string strConstraint = constraint == null ? null : constraint.ToString();
            CityModel[] items = strConstraint == null ? _adapter.Items.ToArray() : _adapter.Items.Where(i => i.Name.ToLowerInvariant().Contains(strConstraint.ToLowerInvariant())).ToArray();
            FilterResults results = new FilterResults()
            {
                Values = items.Select(i => i.Name).ToArray(),
                Count = items.Length
            };
            return results;
        }

        protected override void PublishResults(Java.Lang.ICharSequence constraint, Filter.FilterResults results)
        {
            _adapter.SetFilter(results.Values.ToArray<string>());
        }
    }
}
