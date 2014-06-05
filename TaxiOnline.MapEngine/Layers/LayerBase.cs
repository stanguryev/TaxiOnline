using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxiOnline.ClientInfrastructure.Data;
using TaxiOnline.MapEngine.Composing;
using TaxiOnline.MapEngine.Geometry;
using TaxiOnline.Toolkit.Patterns;

namespace TaxiOnline.MapEngine.Layers
{
    public class LayerBase : DisposableObject
    {
        protected MapPoint _center;
        protected double _zoom;
        protected bool _isInitialized;
        protected BitmapSize _totalSize;
        protected BitmapSize _tileSize;

        public MapPoint Center
        {
            get { return _center; }
            set
            {
                if (!_center.Equals(value))
                {
                    _center = value;
                    OnCenterChanged();
                }
            }
        }

        public double Zoom
        {
            get { return _zoom; }
            set
            {
                if (_zoom != value)
                {
                    _zoom = value;
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
                OnTotalSizeChanged();
            }
        }

        public BitmapSize TileSize
        {
            get { return _tileSize; }
            set { _tileSize = value; }
        }

        public bool IsInitialized
        {
            get { return _isInitialized; }
        }

        public event EventHandler CenterChanged;

        public event EventHandler ZoomChanged;

        public event EventHandler TotalSizeChanged;

        public LayerBase()
        {

        }

        public virtual void Initialize()
        {
            _isInitialized = true;
        }

        protected virtual void OnCenterChanged()
        {
            EventHandler handler = CenterChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        protected virtual void OnZoomChanged()
        {
            EventHandler handler = ZoomChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        protected virtual void OnTotalSizeChanged()
        {
            EventHandler handler = TotalSizeChanged;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }
    }
}
