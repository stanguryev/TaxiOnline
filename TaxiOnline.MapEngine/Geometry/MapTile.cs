using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxiOnline.ClientInfrastructure.Data;

namespace TaxiOnline.MapEngine.Geometry
{
    public struct MapTile : IEquatable<MapTile>
    {
        private int _xNumber;
        private int _yNumber;
        private int _zoom;

        public int XNumber
        {
            get { return _xNumber; }
        }

        public int YNumber
        {
            get { return _yNumber; }
        }

        public int Zoom
        {
            get { return _zoom; }
        }

        public MapTile(MapSquare square, double zoom)
        {
            _zoom = GetTileZoom(zoom);
            _xNumber = GetXFromSquare(square, Math.Pow(2, _zoom));
            _yNumber = GetYFromSquare(square, Math.Pow(2, _zoom));
        }

        public static int GetTileZoom(double zoom)
        {
            return (int)Math.Round(zoom);
        }

        public MapSquare GetActualSquare()
        {
            MapPoint start = GetOffset(_xNumber, _yNumber);
            MapPoint finish = GetOffset(_xNumber + 1, _yNumber + 1);
            return new MapSquare(start.Latitude, finish.Latitude, start.Longitude, finish.Longitude);
        }

        public MapTile Offset(int x, int y)
        {
            return new MapTile
            {
                _xNumber = _xNumber + x,
                _yNumber = _yNumber + y,
                _zoom = _zoom
            };
        }

        public override int GetHashCode()
        {
            return unchecked(_zoom.GetHashCode() + _xNumber.GetHashCode() + _yNumber.GetHashCode());
        }

        public override bool Equals(object obj)
        {
            if (obj is MapTile)
            {
                MapTile otherTile = (MapTile)obj;
                return Equals(otherTile);
            }
            return base.Equals(obj);
        }

        public override string ToString()
        {
            return string.Format("{0}/{1}/{2}", _zoom, _xNumber, _yNumber);
        }

        public bool Equals(MapTile other)
        {
            return _zoom == other._zoom && _xNumber == other._xNumber && _yNumber == other._yNumber;
        }

        public static bool operator ==(MapTile first, MapTile second)
        {
            return first.Equals(second);
        }

        public static bool operator !=(MapTile first, MapTile second)
        {
            return !first.Equals(second);
        }

        private static int GetXFromSquare(MapSquare square, double zoom)
        {
            return (int)Math.Round(((square.LeftLongitude + 180.0) / 360.0) * (double)zoom);
        }

        private static int GetYFromSquare(MapSquare square, double zoom)
        {
            double latitude = square.BottomLatitude * Math.PI / 180.0;
            return (int)Math.Round((1.0f - (System.Math.Log(System.Math.Tan(latitude) + (1.0f / System.Math.Cos(latitude)))) / System.Math.PI) / 2f * (double)zoom);
        }

        private MapPoint GetOffset(int x, int y)
        {
            double angle = Math.PI - ((2.0 * Math.PI * (double)y) / Math.Pow(2.0, (double)_zoom));
            double longitude = (double)(((double)x / Math.Pow(2.0, (double)_zoom) * 360.0) - 180.0);
            double latitude = (double)(180.0 / Math.PI * Math.Atan(Math.Sinh(angle)));
            return new MapPoint(latitude, longitude);
        }
    }
}
