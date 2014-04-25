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
        int LatitudeOffsetToPixels(double from, double to, double longitude);
        int LongitudeOffsetToPixels(double from, double to, double latitude);
    }
}
