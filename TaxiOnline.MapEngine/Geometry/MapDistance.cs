using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxiOnline.ClientInfrastructure.Data;

namespace TaxiOnline.MapEngine.Geometry
{
    public struct MapDistance
    {
        private const double EarthRadius = 6371000;

        private MapPoint _from;
        private MapPoint _to;

        public MapDistance(MapPoint from, MapPoint to)
        {
            _from = from;
            _to = to;
        }

        public double GetInMeters()
        {
            double fromLatitude = _from.Latitude * Math.PI / 180.0;
            double fromLongitude = _from.Longitude * Math.PI / 180.0;
            double toLatitude = _to.Latitude * Math.PI / 180.0;
            double toLongitude = _to.Longitude * Math.PI / 180.0;
            double angle = Math.Pow(Math.Sin(Math.Abs(fromLatitude - toLatitude) / 2), 2) + Math.Cos(fromLatitude) * Math.Cos(toLatitude) * Math.Pow(Math.Sin(Math.Abs(fromLongitude - toLongitude) / 2), 2);
            double relativeDistance = 2 * Math.Atan2(Math.Sqrt(angle), Math.Sqrt(1 - angle));
            return EarthRadius * relativeDistance;
        }
    }
}
