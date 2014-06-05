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

namespace TaxiOnline.MapEngine.Android.Views
{
    public class MapViewAnimator
    {
        private const float MoveThreshold = 1f;
        private readonly MapView _target;
        private global::Android.Views.Animations.Animation _currentAnimation;
        private float _accumulativeXOffset;
        private float _accumulativeYOffset;
        private float _accumulativeScaleFactor;
        //private bool _isDirty;

        //public bool IsDirty
        //{
        //    get { return _isDirty; }
        //    set
        //    {
        //        _isDirty = value;
        //        if (!value)
        //            Reset();
        //    }
        //}

        public float AccumulativeXOffset
        {
            get { return _accumulativeXOffset; }
        }

        public float AccumulativeYOffset
        {
            get { return _accumulativeYOffset; }
        }

        public float AccumulativeScaleFactor
        {
            get { return _accumulativeScaleFactor; }
        }

        public event EventHandler RefreshRequested;

        public MapViewAnimator(MapView target)
        {
            _target = target;
        }

        //private bool CheckScrollRefresh(out float xOffset, out float yOffset)
        //{
        //    if (!_isDirty)
        //    {
        //        xOffset = _accumulativeXOffset;
        //        yOffset = _accumulativeYOffset;
        //        _isDirty = true;
        //        return true;
        //    }
        //    xOffset = yOffset = 0f;
        //    return false;
        //    //if (Math.Abs(_accumulativeXOffset) >= RefreshTresholnd || Math.Abs(_accumulativeYOffset) >= RefreshTresholnd)
        //    //{
        //    //    xOffset = _accumulativeXOffset;
        //    //    yOffset = _accumulativeYOffset;
        //    //    _accumulativeXOffset = _accumulativeYOffset = 0f;
        //    //    _target.ClearAnimation();
        //    //    return true;
        //    //}
        //    //xOffset = yOffset = 0f;
        //    //return false;
        //}

        //private bool CheckForScaleRefresh(float scaleFactor)
        //{
        //    if (!_isDirty)
        //    {
        //        _isDirty = true;
        //        scaleFactor = _accumulativeScaleFactor;
        //        return true;
        //    }
        //    scaleFactor = 1;
        //    return false;
        //}

        public void NotifyScroll(float xOffset, float yOffset)
        {
            //if (_currentAnimation == null)
            //{
            //    _currentAnimation = new global::Android.Views.Animations.TranslateAnimation(0f, 0f, xOffset, yOffset)
            //    {
            //        Duration = 10
            //    };
            //    _currentAnimation.AnimationEnd += CurrentAnimation_AnimationEnd;
            //    _target.StartAnimation(_currentAnimation);
            //}
            _accumulativeXOffset += xOffset;
            _accumulativeYOffset += yOffset;
            CheckRefresh();
            if (!_target.IsDirty)
                _target.Invalidate();
        }

        public void NotifyScale(float scaleFactor)
        {
            _accumulativeScaleFactor *= scaleFactor;
            CheckRefresh();
            if (!_target.IsDirty)
                _target.Invalidate();
        }

        public void NotifyRefreshed()
        {
            Reset();
        }

        private void CheckRefresh()
        {
            if (/*!_isDirty &&*/ (Math.Abs(_accumulativeXOffset) >= MoveThreshold || Math.Abs(_accumulativeYOffset) >= MoveThreshold || Math.Abs(Math.Log(_accumulativeScaleFactor, 2.0)) >= 1.0))
            {
                //_isDirty = true;
                using (Handler handler = new Handler())
                    handler.Post(OnRefreshRequested);
            }
        }

        private void Reset()
        {
            _accumulativeXOffset = _accumulativeYOffset = 0f;
            _accumulativeScaleFactor = 1f;
        }

        protected virtual void OnRefreshRequested()
        {
            EventHandler handler = RefreshRequested;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        private void CurrentAnimation_AnimationEnd(object sender, global::Android.Views.Animations.Animation.AnimationEndEventArgs e)
        {
            if (_currentAnimation != null)
            {
                _currentAnimation.AnimationEnd -= CurrentAnimation_AnimationEnd;
                _currentAnimation.Dispose();
                _currentAnimation = null;
            }
            _target.Invalidate();
        }
    }

}