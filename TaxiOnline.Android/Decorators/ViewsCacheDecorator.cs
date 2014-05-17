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

namespace TaxiOnline.Android.Decorators
{
    public class ViewsCacheDecorator<TModel>
    {
        private readonly Dictionary<TModel, View> _cacheDictionary = new Dictionary<TModel, View>();
        private readonly Func<View> _createViewDelegate;
        private Dictionary<TModel, bool> _itemsUsage = new Dictionary<TModel, bool>();

        public ViewsCacheDecorator(Func<View> createViewDelegate)
        {
            _createViewDelegate = createViewDelegate;
        }

        public void NotifyFillStarted()
        {
            _itemsUsage = _cacheDictionary.Keys.ToDictionary(k => k, k => false);
        }

        public void NotifyFillFinished()
        {
            return;
            View[] oldViews = _itemsUsage.Where(kv => !kv.Value).Select(kv => kv.Key).Join(_cacheDictionary, m => m, kv => kv.Key, (m, kv) => kv.Value).ToArray();
            if (oldViews.Length > 0)
            {
                foreach (View oldView in oldViews)
                    oldView.Dispose();
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }
        }

        public View GetView(TModel model)
        {
            if (!_cacheDictionary.ContainsKey(model))
                _cacheDictionary.Add(model, _createViewDelegate());
            if (_itemsUsage != null && _itemsUsage.ContainsKey(model))
                _itemsUsage[model] = true;
            return _cacheDictionary[model];
        }

        public View GetCachedView(TModel model)
        {
            return _cacheDictionary.ContainsKey(model) ? _cacheDictionary[model] : null;
        }
    }
}