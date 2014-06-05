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
using Android.Graphics;
using TaxiOnline.MapEngine.Composing;
using TaxiOnline.MapEngine.Layers;
using System.Threading;

namespace TaxiOnline.MapEngine.Android.Composing
{
    public class AndroidComposer : ComposerBase<byte[], Bitmap>
    {
        public AndroidComposer(TileLayerBase<byte[]> tileLayer)
            : base(tileLayer)
        {

        }

        protected override Bitmap ReadBitmap(byte[] raw)
        {
            return BitmapFactory.DecodeByteArray(raw, 0, raw.Length);
        }

        protected override void DisposeOldBitmap()
        {
            if (_bitmap == null)
                return;
            _bitmap.Dispose();
            _bitmap = null;
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        protected override void ComposeCore(Func<IEnumerable<BitmapTile>> tiles, Func<BitmapTile, System.Threading.Tasks.Task<Bitmap>> fetchDelegate, Action<Bitmap> endCallback, CancellationToken cancellation)
        {
            ComposeImpl(tiles, fetchDelegate, endCallback, cancellation);
        }

        protected async void ComposeImpl(Func<IEnumerable<BitmapTile>> tilesDelegate, Func<BitmapTile, System.Threading.Tasks.Task<Bitmap>> fetchDelegate, Action<Bitmap> endCallback, CancellationToken cancellation)
        {
            Bitmap outResult = global::Android.Graphics.Bitmap.CreateBitmap(_tileLayer.TotalSize.Width, _tileLayer.TotalSize.Height, global::Android.Graphics.Bitmap.Config.Rgb565);
            using (Canvas drawingContext = new Canvas(outResult))
            {
                IEnumerable<BitmapTile> tiles;
                int composedPartsCount = 0;
                while ((tiles = tilesDelegate()).Count() > 0)
                    foreach (BitmapTile tile in tiles)
                    {
                        if (cancellation.IsCancellationRequested)
                        {
                            if (composedPartsCount > 0)
                                endCallback(outResult);
                            return;
                        }
                        Bitmap bitmap = await fetchDelegate(tile);
                        if (bitmap == null)
                        {
                            await System.Threading.Tasks.Task.Delay(2000);//, cancellation);
                            continue;
                        }
                        using (global::Android.Graphics.Rect sourceRect = new global::Android.Graphics.Rect(0, 0, _tileLayer.TileSize.Width, _tileLayer.TileSize.Height))
                        using (global::Android.Graphics.Rect targetRect = new global::Android.Graphics.Rect(tile.Bounds.Left, tile.Bounds.Top, tile.Bounds.Left + tile.Bounds.Width, tile.Bounds.Top + tile.Bounds.Height))
                        using (global::Android.Graphics.Paint paint = new global::Android.Graphics.Paint())
                            drawingContext.DrawBitmap(bitmap, sourceRect, targetRect, paint);
                        composedPartsCount++;
                        bitmap.Dispose();
                        bitmap = null;
                        GC.Collect();
                        GC.WaitForPendingFinalizers();
                        endCallback(outResult);
                    }
            }
        }
    }
}