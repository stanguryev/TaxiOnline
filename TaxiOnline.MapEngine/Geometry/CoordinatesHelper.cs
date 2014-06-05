using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaxiOnline.MapEngine.Geometry
{
    public static class CoordinatesHelper
    {
        public static double GetTotalLatitudeDegrees(double zoom)
        {
            return 90.0 / Math.Pow(2.0, zoom);
        }

        public static double GetTotalLongitudeDegrees(double zoom)
        {
            return 180.0 / Math.Pow(2.0, zoom);
        }

        public static double GetTilesFromLatitude(double latitude, double zoom)
        {
            double tangent = Math.Tan(latitude * Math.PI / 180.0);
            double arcsine = Math.Log(tangent + Math.Sqrt(tangent * tangent + 1.0));
            return Math.Pow(2.0, zoom) / 2.0 / Math.PI * (Math.PI - arcsine);
        }
    }
}
