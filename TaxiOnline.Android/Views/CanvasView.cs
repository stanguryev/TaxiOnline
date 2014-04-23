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
using Android.Util;
using Android.Database;

namespace TaxiOnline.Android.Views
{
    public class CanvasView : AdapterView<IAdapter>
    {
        private class CanvasViewDataSetObserver : DataSetObserver
        {
            private readonly CanvasView _canvasView;

            public CanvasViewDataSetObserver(CanvasView canvasView)
            {
                _canvasView = canvasView;
            }

            public override void OnChanged()
            {
                base.OnChanged();
                _canvasView.RequestLayout();
            }

            public override void OnInvalidated()
            {
                base.OnInvalidated();
            }
        }

        private IAdapter _adapter;

        public override IAdapter Adapter
        {
            get { return _adapter; }
            set
            {
                _adapter = value;
                _adapter.RegisterDataSetObserver(new CanvasViewDataSetObserver(this));
                RemoveAllViewsInLayout();
                RequestLayout();
            }
        }

        public override View SelectedView
        {
            get { return null; }
        }

        public CanvasView(Context context, IAttributeSet attrs) :
            base(context, attrs)
        {

        }

        public CanvasView(Context context, IAttributeSet attrs, int defStyle) :
            base(context, attrs, defStyle)
        {

        }

        protected override void OnMeasure(int widthMeasureSpec, int heightMeasureSpec)
        {
            base.OnMeasure(widthMeasureSpec, heightMeasureSpec);
            //for (int i = 0; i < ChildCount; i++)
            //{
            //    View currentView = GetChildAt(i);
            //    currentView.Measure(widthMeasureSpec, heightMeasureSpec);
            //}
        }

        protected override void OnLayout(bool changed, int left, int top, int right, int bottom)
        {
            base.OnLayout(changed, left, top, right, bottom);
            if (_adapter == null)
                return;
            for (int i = 0; i < _adapter.Count; i++)
            {
                View currentView = _adapter.GetView(i, null, this);
                AddViewInLayout(currentView, i, currentView.LayoutParameters, true);
                currentView.Measure(MeasureSpec.MakeMeasureSpec(Width, MeasureSpecMode.AtMost), MeasureSpec.MakeMeasureSpec(Height, MeasureSpecMode.AtMost));
            }
            for (int i = 0; i < ChildCount; i++)
            {
                View currentView = GetChildAt(i);
                AbsoluteLayout.LayoutParams layoutParameters = (AbsoluteLayout.LayoutParams)currentView.LayoutParameters;
                int viewLeft = layoutParameters.X;
                int viewTop = layoutParameters.Y;
                int viewRight = layoutParameters.X + layoutParameters.Width;
                int viewBottom = layoutParameters.Y + layoutParameters.Height;
                currentView.Layout(viewLeft, viewTop, viewRight, viewBottom);
            }
            //Invalidate();
        }

        public override void SetSelection(int position)
        {

        }
    }
}