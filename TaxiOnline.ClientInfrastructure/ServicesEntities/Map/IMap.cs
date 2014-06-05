using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxiOnline.ClientInfrastructure.Data;

namespace TaxiOnline.ClientInfrastructure.ServicesEntities.Map
{
    public interface IMap
    {
        MapPoint MapCenter { get; set; }
        double MapZoom { get; set; }
        event EventHandler MapChanged;
        event EventHandler MapCenterChanged;
        event EventHandler MapZoomChanged;
        //int LatitudeOffsetToPixels(double from, double to, double longitude);
        //int LongitudeOffsetToPixels(double from, double to, double latitude);
        bool GetPixelsFromCoordinates(MapPoint coordinates, out int x, out int y);
    }
}
