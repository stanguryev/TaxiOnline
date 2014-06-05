using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using TaxiOnline.ClientInfrastructure.Data;
using TaxiOnline.MapEngine.Cache;
using TaxiOnline.MapEngine.Common;
using TaxiOnline.MapEngine.Composing;
using TaxiOnline.MapEngine.Geometry;
using TaxiOnline.MapEngine.Providers;

namespace TaxiOnline.MapEngine.Layers
{
    public class TileLayerBase<TBitmap> : LayerBase where TBitmap : class
    {
        private readonly TileProviderBase<TBitmap> _tileProvider;
        private CacheBase<MapTile, TBitmap> _cache;
        private Lazy<ObservableBitmapTileSet> _tileSet;
        private bool _areTilesChanged;

        public IEnumerable<BitmapTile> Bitmaps
        {
            get { return _tileSet.Value.Bitmaps; }
        }

        public bool AreTilesChanged
        {
            get { return _areTilesChanged; }
            set { _areTilesChanged = value; }
        }

        public event NotifyCollectionChangedEventHandler BitmapsCollectionChanged
        {
            add { _tileSet.Value.BitmapsCollectionChanged += value; }
            remove { _tileSet.Value.BitmapsCollectionChanged -= value; }
        }

        public TileLayerBase(IMapSupplyFactory<TBitmap> supplyFactory, TileProviderBase<TBitmap> tileProvider)
        {
            _tileProvider = tileProvider;
            _cache = supplyFactory.GetTilesCache(30, tile => tileProvider.GetBitmap(tile));
            _tileSet = new Lazy<ObservableBitmapTileSet>(() => new ObservableBitmapTileSet(_center, _zoom, TotalSize, TileSize, new ObservableMapTileSet(_center.Latitude, _center.Longitude, _zoom)));
        }

        public override void Initialize()
        {
            _tileSet.Value.BitmapsCollectionChanged += Value_BitmapsCollectionChanged;
            _tileSet.Value.Zoom(_zoom);
            base.Initialize();
        }

        public BitmapSize? GetOffsetFromCoordinates(MapPoint coordinates)
        {
            return _tileSet.Value.GetOffsetFromCoordinates(coordinates);
        }

        public async System.Threading.Tasks.Task<TBitmap> GetTileBitmap(Geometry.MapTile tile)
        {
            return await _cache.GetItem(tile); //System.Threading.Tasks.Task.Run(() => _cache[tile]);
        }

        public bool MoveCenterForView(float xOffset, float yOffset)
        {
            MapPoint? newCenter = _tileSet.Value.MoveCenterForView(xOffset, yOffset);
            if (newCenter.HasValue)
            {
                _center = newCenter.Value;
                OnCenterChanged();
                return true;
            }
            return false;
        }

        protected override void DisposeManagedResources()
        {
            base.DisposeManagedResources();
            _cache.Dispose();
        }

        protected override void OnCenterChanged()
        {
            if (_isInitialized)
                _tileSet.Value.MoveCenter(_center.Latitude, _center.Longitude);
            base.OnCenterChanged();
        }

        protected override void OnZoomChanged()
        {
            if (_isInitialized)
                _tileSet.Value.Zoom(_zoom);
            base.OnZoomChanged();
        }

        protected override void OnTotalSizeChanged()
        {
            if (_isInitialized)
                _tileSet.Value.UpdateTotalSize(_totalSize);
            base.OnTotalSizeChanged();
        }

        private void Value_BitmapsCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            _areTilesChanged = true;
        }
    }
}
