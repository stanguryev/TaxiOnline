using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxiOnline.ClientInfrastructure.Data;
using TaxiOnline.MapEngine.Composing;
using TaxiOnline.MapEngine.Layers;
using TaxiOnline.Toolkit.Patterns;

namespace TaxiOnline.MapEngine.Common
{
    public abstract class MapBase : DisposableObject
    {
        public abstract MapPoint Center { get; set; }

        public abstract double Zoom { get; set; }

        public event EventHandler CenterChanged;

        public event EventHandler TotalSizeChanged;

        public event EventHandler ZoomChanged;

        public MapBase()
        {

        }

        public abstract LayerBase AddHttpLayer(string urlTemplate);

        public abstract BitmapSize? GetOffsetFromCoordinates(MapPoint coordinates);

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
