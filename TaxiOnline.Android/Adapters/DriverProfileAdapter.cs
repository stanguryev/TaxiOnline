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
        private Dictionary<PedestrianRequestModel, PopupWindow> _pedestrianInfoPopups = new Dictionary<PedestrianRequestModel, PopupWindow>();

        public DriverProfileAdapter(Activity context, DriverProfileModel model)
        {
            _context = context;
            _model = model;
            _viewCache = new ViewsCacheDecorator<PedestrianModel>(() => _context.LayoutInflater.Inflate(Resource.Layout.PedestrianInfoLayout, null, false));
            _selfItemView = new Lazy<View>(() => _context.LayoutInflater.Inflate(Resource.Layout.DriverSelfInfoLayout, null, false));
            model.PedestriansChanged += Model_PedestriansChanged;
            model.PedestriansCollectionChanged += Model_PedestriansCollectionChanged;
            model.PedestrianRequestsChanged += Model_PedestrianRequestsChanged;
            model.PedestrianRequestsCollectionChanged += Model_PedestrianRequestsCollectionChanged;
            model.CurrentLocationChanged += Model_CurrentLocationChanged;
            model.Map.MapService.Map.MapCenterChanged += Map_MapCenterChanged;
            model.Map.MapService.Map.MapZoomChanged += Map_MapZoomChanged;
            UpdatePedestrians();
            ShowAllPedestriansInfo();
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
            ImageView pedestrianCallIcon = view.FindViewById<ImageView>(Resource.Id.pedestrianCallIcon);
            pedestrianModel.MadeCall += (sender, e) => pedestrianCallIcon.Visibility = ViewStates.Visible;
        }

        private void UpdatePedestrians()
        {
            IEnumerable<PedestrianModel> modelCollection = _model.Pedestrians;
            _items = modelCollection == null ? new List<PedestrianModel>() : modelCollection.ToList();
            NotifyDataSetChanged();
        }

        private void ShowPedestrianInfoPopupWindow(PedestrianRequestModel request)
        {
            View pedestrianView = _viewCache.GetCachedView(request.RequestAuthor);
            if (pedestrianView != null && !_pedestrianInfoPopups.ContainsKey(request))
            {
                PopupWindow pedestrianInfoPopup = new PopupWindow(_context.LayoutInflater.Inflate(Resource.Layout.PedestrianPopupDetailsLayout, null), 100, 100);
                _pedestrianInfoPopups.Add(request, pedestrianInfoPopup);
                pedestrianInfoPopup.SetBackgroundDrawable(_context.Resources.GetDrawable(Resource.Drawable.PedestrianInfoPopup));
                HookModelToDetailsPopupWindow(pedestrianInfoPopup, request);
                pedestrianInfoPopup.ShowAsDropDown(pedestrianView, -32, 0);
                pedestrianInfoPopup.Update();
            }
        }

        private void ClosePedestrianInfoPopupWindow(PedestrianRequestModel request)
        {
            if (!_pedestrianInfoPopups.ContainsKey(request))
                return;
            PopupWindow pedestrianInfoPopup = _pedestrianInfoPopups[request];
            UnhookModelFromDetailsPopupWindow(pedestrianInfoPopup, request);
            _pedestrianInfoPopups.Remove(request);
            pedestrianInfoPopup.Dismiss();
            pedestrianInfoPopup.Dispose();
        }

        private void ShowAllPedestriansInfo()
        {
            IEnumerable<PedestrianRequestModel> requests = _model.PedestrianRequests;
            if (requests != null)
                foreach (PedestrianRequestModel request in requests)
                    ShowPedestrianInfoPopupWindow(request);
        }

        private void ClearPedestriansInfo()
        {
            foreach (PedestrianRequestModel request in _pedestrianInfoPopups.Keys.ToArray())
                ClosePedestrianInfoPopupWindow(request);
        }

        private void HookModelToDetailsPopupWindow(PopupWindow pedestrianInfoPopup, PedestrianRequestModel request)
        {
            TextView pedestrianRequestTextView = pedestrianInfoPopup.ContentView.FindViewById<TextView>(Resource.Id.pedestrianRequestTextView);
            pedestrianRequestTextView.Text = request.Comment;
            LinearLayout pedestrianRequestLayout = pedestrianInfoPopup.ContentView.FindViewById<LinearLayout>(Resource.Id.pedestrianRequestLayout);
            pedestrianRequestLayout.Click += (sender, e) =>
            {
                _model.SelectedPedestrianRequest = request;
                UIHelper.GoResultActivity(_context, typeof(DriverProfileResponseActivity), 1);
            };
        }

        private void UnhookModelFromDetailsPopupWindow(PopupWindow pedestrianInfoPopup, PedestrianRequestModel request)
        {

        }

        private void Model_PedestriansChanged(object sender, EventArgs e)
        {
            _context.RunOnUiThread(UpdatePedestrians);
        }

        private void Model_PedestriansCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            _context.RunOnUiThread(() => ObservableCollectionHelper.ApplyChanges(e, _items));
        }

        private void Model_PedestrianRequestsChanged(object sender, EventArgs e)
        {
            _context.RunOnUiThread(() =>
            {
                ClearPedestriansInfo();
                ShowAllPedestriansInfo();
            });
        }

        private void Model_PedestrianRequestsCollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            _context.RunOnUiThread(() =>
            {
                if (e.OldItems != null)
                    foreach (PedestrianRequestModel request in e.OldItems.OfType<PedestrianRequestModel>().ToArray())
                        ClosePedestrianInfoPopupWindow(request);
                if (e.NewItems != null)
                    foreach (PedestrianRequestModel request in e.NewItems.OfType<PedestrianRequestModel>().ToArray())
                        ShowPedestrianInfoPopupWindow(request);
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