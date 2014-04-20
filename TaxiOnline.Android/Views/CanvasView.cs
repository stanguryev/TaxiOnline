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

namespace TaxiOnline.Android.Views
{
    public class CanvasView : AdapterView<IAdapter>
    {
        private IAdapter _adapter;

        public override IAdapter Adapter
        {
            get { return _adapter; }
            set
            {
                _adapter = value;
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
        }

        protected override void OnLayout(bool changed, int left, int top, int right, int bottom)
        {
            base.OnLayout(changed, left, top, right, bottom);
            if (_adapter == null)
                return;
            for (int i = 0; i < _adapter.Count; i++)
            {
                View currentView = _adapter.GetView(i, null, this);
                AbsoluteLayout.LayoutParams layoutParameters = (AbsoluteLayout.LayoutParams)currentView.LayoutParameters;
                int viewLeft = layoutParameters.X;
                int viewTop = layoutParameters.Y;
                int viewRight = layoutParameters.X + layoutParameters.Width;
                int viewBottom = layoutParameters.Y + layoutParameters.Height;
                currentView.Layout(viewLeft, viewTop, viewRight, viewBottom);
            }
            Invalidate();
        }

        public override void SetSelection(int position)
        {
            
        }
    }
}