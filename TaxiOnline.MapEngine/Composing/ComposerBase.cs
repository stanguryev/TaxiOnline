using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using TaxiOnline.MapEngine.Layers;
using TaxiOnline.Toolkit.Patterns;

namespace TaxiOnline.MapEngine.Composing
{
    public abstract class ComposerBase<TRawBitmap, TBitmap> : DisposableObject
        where TRawBitmap : class
        where TBitmap : class
    {
        protected TileLayerBase<TRawBitmap> _tileLayer;
        protected TBitmap _bitmap;

        public TBitmap Bitmap
        {
            get { return _bitmap; }
        }

        public event EventHandler BitmapChanged;

        public ComposerBase(TileLayerBase<TRawBitmap> tileLayer)
        {
            _tileLayer = tileLayer;
        }

        public void Compose(CancellationToken cancellation)
        {
            if (_tileLayer.TotalSize.Width * _tileLayer.TotalSize.Height > 0)
            {
                List<BitmapTile> tiles = _tileLayer.Bitmaps.ToList();
                if (tiles.Count > 0)
                    ComposeCore(() => tiles.ToArray(), tile => FetchBitmap(tile, tiles), bitmap =>
                    {
                        if (_bitmap != bitmap)
                        {
                            DisposeOldBitmap();
                            _bitmap = bitmap;
                        }
                        OnBitmapChanged();
                    }, cancellation);
            }
        }

        public void RemoveBitmap()
        {
            DisposeOldBitmap();
        }

        private async Task<TBitmap> FetchBitmap(BitmapTile tile, IList<BitmapTile> tilesList)
        {
            TRawBitmap rawBitmap = await _tileLayer.GetTileBitmap(tile.Tile);
            if (rawBitmap != null)
                tilesList.Remove(tile);
            return rawBitmap == null ? null : ReadBitmap(rawBitmap);
        }

        protected abstract TBitmap ReadBitmap(TRawBitmap raw);

        protected abstract void ComposeCore(Func<IEnumerable<BitmapTile>> tiles, Func<BitmapTile, Task<TBitmap>> fetchDelegate, Action<TBitmap> endCallback, CancellationToken cancellation);

        protected abstract void DisposeOldBitmap();

        protected override void DisposeManagedResources()
        {
            base.DisposeManagedResources();
            DisposeOldBitmap();
        }

        protected virtual void OnBitmapChanged()
        {
            EventHandler handler = BitmapChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }
    }
}
