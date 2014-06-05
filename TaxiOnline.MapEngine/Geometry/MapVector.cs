using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaxiOnline.MapEngine.Geometry
{
    public struct MapVector
    {
        private double _latitudeOffset;
        private double _longitudeOffset;

        public double LatitudeOffset
        {
            get { return _latitudeOffset; }
            set { _latitudeOffset = value; }
        }

        public double LongitudeOffset
        {
            get { return _longitudeOffset; }
            set { _longitudeOffset = value; }
        }

        public MapVector(double latitudeOffset, double longitudeOffset)
        {
            _latitudeOffset = latitudeOffset;
            _longitudeOffset = longitudeOffset;
        }

        public static MapVector operator -(MapVector vector)
        {
            return new MapVector(-vector._latitudeOffset, -vector._longitudeOffset);
        }
    }
}
