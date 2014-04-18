using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaxiOnline.ClientInfrastructure.Data
{
    public struct MapPoint
    {
        private double _latitude;
        private double _longitude;

        public MapPoint(double latitude, double longitude)
        {
            _latitude = latitude;
            _longitude = longitude;
        }

        public double Latitude
        {
            get { return _latitude; }
            set { _latitude = value; }
        }

        public double Longitude
        {
            get { return _longitude; }
            set { _longitude = value; }
        }
    }
}
