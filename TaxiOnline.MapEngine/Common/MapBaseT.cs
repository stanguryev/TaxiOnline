using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TaxiOnline.ClientInfrastructure.Data;
using TaxiOnline.MapEngine.Composing;
using TaxiOnline.MapEngine.Geometry;
using TaxiOnline.MapEngine.Layers;
using TaxiOnline.Toolkit.Patterns;

namespace TaxiOnline.MapEngine.Common
{
    public abstract class MapBase<TRawBitmap, TBitmap> : MapBase
        where TRawBitmap : class
        where TBitmap : class
    {
        private readonly IMapSupplyFactory<TRawBitmap, TBitmap> _supplyFactory;
        private readonly ConcurrentDictionary<TileLayerBase<TRawBitmap>, ComposerBase<TRawBitmap, TBitmap>> _composers = new ConcurrentDictionary<TileLayerBase<TRawBitmap>, ComposerBase<TRawBitmap, TBitmap>>();
        private TileLayerBase<TRawBitmap> _currentLayer;
        protected MapPoint _center;
        protected double _zoom;
        protected BitmapSize _totalSize;
        protected BitmapSize _tileSize;
        private CancellationTokenSource _cancellation;
        private Action _pendingRequest;
        //private Action _nextRequest;
        //private readonly object _requestsLocker = new object();

        public TBitmap CurrentBitmap
        {
            get { return _currentLayer == null ? null : _composers[_currentLayer].Bitmap; }
        }

        public override MapPoint Center
        {
            get { return _center; }
            set
            {
                if (!_center.Equals(value))
                {
                    _center = value;
                    UpdateBitmap();
                    OnCenterChanged();
                }
            }
        }

        public override double Zoom
        {
            get { return _zoom; }
            set
            {
                if (_zoom != value)
                {
                    _zoom = value;
                    UpdateBitmap();
                    OnZoomChanged();
                }
            }
        }

        public BitmapSize TotalSize
        {
            get { return _totalSize; }
            set
            {
                _totalSize = value;
                UpdateBitmap();
                OnTotalSizeChanged();
            }
        }

        public BitmapSize TileSize
        {
            get { return _tileSize; }
            set { _tileSize = value; }
        }

        public event EventHandler CurrentBitmapChanged;

        public MapBase(IMapSupplyFactory<TRawBitmap, TBitmap> supplyFactory)
        {
            _supplyFactory = supplyFactory;
        }

        public override LayerBase AddHttpLayer(string urlTemplate)
        {
            _currentLayer = new HttpTileLayer<TRawBitmap>(_supplyFactory, urlTemplate);
            _composers.TryAdd(_currentLayer, _supplyFactory.GetTilesComposer(_currentLayer));
            _composers[_currentLayer].BitmapChanged += (sender, e) =>
            {
                OnCurrentBitmapChanged();
                CheckPendingRequest();
            };
            UpdateBitmap();
            return _currentLayer;
        }

        public bool MoveCenterForView(float xOffset, float yOffset)
        {
            if (_currentLayer != null && _currentLayer.MoveCenterForView(xOffset, yOffset))
            {
                _center = _currentLayer.Center;
                UpdateBitmap();
                OnCenterChanged();
                return true;
            }
            return false;
        }

        public void MoveCenterForViewAndScaleSequentially(float xOffset, float yOffset, double newZoom)
        {
            if (_pendingRequest == null)
                MoveCenterForViewAndScale(xOffset, yOffset, newZoom);
            else
                _pendingRequest = () => MoveCenterForViewAndScale(xOffset, yOffset, newZoom);
        }

        public bool MoveCenterForViewAndScale(float xOffset, float yOffset, double newZoom)
        {
            bool outResult = false;
            if (_currentLayer != null && _currentLayer.MoveCenterForView(xOffset, yOffset))
            {
                _center = _currentLayer.Center;
                OnCenterChanged();
                outResult = true;
            }
            if (_zoom != newZoom)
            {
                _zoom = newZoom;
                OnZoomChanged();
                outResult = true;
            }
            if (outResult)
                UpdateBitmap();
            return outResult;
        }

        public void Clear()
        {
            if (_currentLayer != null)
                _composers[_currentLayer].RemoveBitmap();
        }

        public override BitmapSize? GetOffsetFromCoordinates(MapPoint coordinates)
        {
            return _currentLayer == null ? null : (BitmapSize?)_currentLayer.GetOffsetFromCoordinates(coordinates);
        }

        protected override void DisposeManagedResources()
        {
            base.DisposeManagedResources();
            foreach (KeyValuePair<TileLayerBase<TRawBitmap>, ComposerBase<TRawBitmap, TBitmap>> composer in _composers)
            {
                composer.Key.Dispose();
                composer.Value.Dispose();
            }
        }

        private async void CheckPendingRequest()
        {
            await Task.Delay(100);
            if (_pendingRequest != null)
            {
                _pendingRequest();
                _pendingRequest = null;
            }
        }

        private void UpdateBitmap()
        {
            if (_currentLayer != null)
            {
                if (_cancellation != null)
                {
                    _cancellation.Cancel();
                    _cancellation.Dispose();
                    _cancellation = null;
                }
                _currentLayer.Center = _center;
                _currentLayer.Zoom = _zoom;
                //if (!_currentLayer.AreTilesChanged)
                //    return;
                //_currentLayer.AreTilesChanged = false;
                if (Math.Abs(_currentLayer.TotalSize.Width * _currentLayer.TotalSize.Width - _totalSize.Width * _totalSize.Height) > 1e-3)
                    _currentLayer.TotalSize = _totalSize;
                _currentLayer.TileSize = _tileSize;
                if (!_currentLayer.IsInitialized)
                    _currentLayer.Initialize();
                _cancellation = new CancellationTokenSource();
                _composers[_currentLayer].Compose(_cancellation.Token);
            }
        }

        protected virtual void OnCurrentBitmapChanged()
        {
            EventHandler handler = CurrentBitmapChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }
    }
}
