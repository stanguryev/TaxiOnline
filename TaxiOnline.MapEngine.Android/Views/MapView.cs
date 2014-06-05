using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;
using TaxiOnline.MapEngine.Android.Common;

namespace TaxiOnline.MapEngine.Android.Views
{
    public class MapView : View, ScaleGestureDetector.IOnScaleGestureListener, GestureDetector.IOnGestureListener, global::Android.Views.View.IOnTouchListener
    {
        private AndroidMap _map;
        private MapViewAnimator _scrollAnimator;
        private ScaleGestureDetector _scaleGestureDetector;
        private GestureDetector _gestureDetector;

        public AndroidMap Map
        {
            get { return _map; }
            set
            {
                UnhookMap();
                _map = value;
                HookMap();
            }
        }

        public MapView(Context context, IAttributeSet attrs) :
            base(context, attrs)
        {
            Initialize();
        }

        public MapView(Context context, IAttributeSet attrs, int defStyle) :
            base(context, attrs, defStyle)
        {
            Initialize();
        }

        protected override void Dispose(bool disposing)
        {
            base.Dispose(disposing);
            UnhookMap();
            if (_scaleGestureDetector != null)
                _scaleGestureDetector.Dispose();
        }

        public override bool OnTouchEvent(MotionEvent e)
        {
            return true;
        }

        private void Initialize()
        {
            _gestureDetector = new GestureDetector(this);
            _scaleGestureDetector = new ScaleGestureDetector(Context, this);
            _scrollAnimator = new MapViewAnimator(this);
            _scrollAnimator.RefreshRequested += ScrollAnimator_RefreshRequested;
            SetOnTouchListener(this);
        }

        protected override void OnDraw(global::Android.Graphics.Canvas canvas)
        {
            base.OnDraw(canvas);
            if (_map != null && _map.CurrentBitmap != null && _map.CurrentBitmap.Handle != IntPtr.Zero)
            {
                int sourceWidth = _map.CurrentBitmap.Width;
                int sourceHeight = _map.CurrentBitmap.Height;
                int x = (int)Math.Round(-_scrollAnimator.AccumulativeXOffset * _scrollAnimator.AccumulativeScaleFactor);
                int y = (int)Math.Round(-_scrollAnimator.AccumulativeYOffset * _scrollAnimator.AccumulativeScaleFactor);
                int width = (int)Math.Round(sourceWidth * _scrollAnimator.AccumulativeScaleFactor);
                int height = (int)Math.Round(sourceHeight * _scrollAnimator.AccumulativeScaleFactor);
                using (global::Android.Graphics.Rect sourceRect = new global::Android.Graphics.Rect(0, 0, sourceWidth, sourceHeight))
                using (global::Android.Graphics.Rect targetRect = new global::Android.Graphics.Rect(x, y, x + width, y + height))
                using (global::Android.Graphics.Paint paint = new global::Android.Graphics.Paint())
                    canvas.DrawBitmap(_map.CurrentBitmap, sourceRect, targetRect, paint);
            }
            //canvas.DrawBitmap(_map.CurrentBitmap, -_scrollAnimator.AccumulativeXOffset, -_scrollAnimator.AccumulativeYOffset, new global::Android.Graphics.Paint());
        }

        protected override void OnSizeChanged(int w, int h, int oldw, int oldh)
        {
            base.OnSizeChanged(w, h, oldw, oldh);
            _map.TotalSize = new MapEngine.Composing.BitmapSize(w, h);
        }

        private void HookMap()
        {
            if (_map == null)
                return;
            _map.CurrentBitmapChanged += Map_CurrentBitmapChanged;
            _map.TotalSize = new MapEngine.Composing.BitmapSize(Width, Height);
        }

        private void UnhookMap()
        {
            if (_map == null)
                return;
            _map.CurrentBitmapChanged -= Map_CurrentBitmapChanged;
        }

        private void Map_CurrentBitmapChanged(object sender, EventArgs e)
        {
            //_scrollAnimator.IsDirty = false;
            _scrollAnimator.NotifyRefreshed();
            Invalidate();
        }

        public bool OnScale(ScaleGestureDetector detector)
        {
            _scrollAnimator.NotifyScale(detector.ScaleFactor);
            return true;
        }

        public bool OnScaleBegin(ScaleGestureDetector detector)
        {
            return true;
        }

        public void OnScaleEnd(ScaleGestureDetector detector)
        {

        }

        public bool OnDown(MotionEvent e)
        {
            return false;
        }

        public bool OnFling(MotionEvent e1, MotionEvent e2, float velocityX, float velocityY)
        {
            return false;
        }

        public void OnLongPress(MotionEvent e)
        {

        }

        public bool OnScroll(MotionEvent e1, MotionEvent e2, float distanceX, float distanceY)
        {
            _scrollAnimator.NotifyScroll(distanceX, distanceY);
            //float xOffset, yOffset;
            //if (_scrollAnimator.CheckScrollRefresh(out xOffset, out yOffset))
            //    using (global::Android.OS.Handler handler = new Handler())
            //        handler.Post(() => _map.MoveCenterForView(xOffset, yOffset));
            return true;
        }

        public void OnShowPress(MotionEvent e)
        {

        }

        public bool OnSingleTapUp(MotionEvent e)
        {
            return false;
        }

        public bool OnTouch(View v, MotionEvent e)
        {
            _scaleGestureDetector.OnTouchEvent(e);
            _gestureDetector.OnTouchEvent(e);
            return false;
        }

        private void ScrollAnimator_RefreshRequested(object sender, EventArgs e)
        {
            _map.MoveCenterForViewAndScaleSequentially(_scrollAnimator.AccumulativeXOffset, _scrollAnimator.AccumulativeYOffset, _map.Zoom + Math.Log(_scrollAnimator.AccumulativeScaleFactor, 2.0));
        }
    }
}