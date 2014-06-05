using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxiOnline.ClientInfrastructure.Data;

namespace TaxiOnline.MapEngine.Geometry
{
    public struct MapSquare
    {
        private double _topLatitude;
        private double _bottomLatitude;
        private double _leftLongitude;
        private double _rightLongitude;

        public double TopLatitude
        {
            get { return _topLatitude; }
        }

        public double BottomLatitude
        {
            get { return _bottomLatitude; }
        }

        public double LeftLongitude
        {
            get { return _leftLongitude; }
        }

        public double RightLongitude
        {
            get { return _rightLongitude; }
        }

        public MapPoint LeftTop
        {
            get { return new MapPoint(_topLatitude, _leftLongitude); }
        }

        public MapPoint LeftBottom
        {
            get { return new MapPoint(_bottomLatitude, _leftLongitude); }
        }

        public MapPoint Center
        {
            get { return new MapPoint((_topLatitude + _bottomLatitude) / 2.0, (_leftLongitude + _rightLongitude) / 2.0); }
        }

        public MapVector Size
        {
            get { return new MapVector(_topLatitude - _bottomLatitude, _rightLongitude - _leftLongitude); }
        }

        public MapVector LongitudeSize
        {
            get { return new MapVector(0, _rightLongitude - _leftLongitude); }
        }

        public MapVector LatitudeSize
        {
            get { return new MapVector(_topLatitude - _bottomLatitude, 0); }
        }

        public MapSquare(double topLatitude, double bottomLatitude, double leftLongitude, double rightLongitude)
        {
            _topLatitude = topLatitude;
            _bottomLatitude = bottomLatitude;
            _leftLongitude = leftLongitude;
            _rightLongitude = rightLongitude;
        }

        public MapSquare(double centerLatitude, double centerLongitude, double zoom)
        {
            double totalLatitudeDegrees = CoordinatesHelper.GetTotalLatitudeDegrees(zoom);
            double totalLongitueDegrees = CoordinatesHelper.GetTotalLongitudeDegrees(zoom);
            _topLatitude = centerLatitude + totalLatitudeDegrees / 2.0;
            _bottomLatitude = centerLatitude - totalLatitudeDegrees / 2.0;
            _leftLongitude = centerLongitude - totalLongitueDegrees / 2.0;
            _rightLongitude = centerLongitude + totalLongitueDegrees / 2.0;
        }

        public static MapSquare operator +(MapSquare thisSquare, MapVector offset)
        {
            return new MapSquare(thisSquare.TopLatitude + offset.LatitudeOffset, thisSquare.LeftLongitude + offset.LongitudeOffset, thisSquare.BottomLatitude + offset.LatitudeOffset, thisSquare.RightLongitude + offset.LongitudeOffset);
        }

        public static MapSquare operator -(MapSquare thisSquare, MapVector offset)
        {
            return new MapSquare(thisSquare.TopLatitude - offset.LatitudeOffset, thisSquare.LeftLongitude - offset.LongitudeOffset, thisSquare.BottomLatitude - offset.LatitudeOffset, thisSquare.RightLongitude - offset.LongitudeOffset);
        }

        public bool Intersects(MapSquare otherSquare)
        {
            return (_topLatitude <= otherSquare.BottomLatitude && _bottomLatitude >= otherSquare.TopLatitude) && (_rightLongitude <= otherSquare.LeftLongitude && _leftLongitude >= otherSquare.RightLongitude);
        }

        public bool Contains(MapSquare otherSquare)
        {
            return _topLatitude >= otherSquare.TopLatitude && _bottomLatitude <= otherSquare.BottomLatitude && _leftLongitude <= otherSquare.LeftLongitude && _rightLongitude >= otherSquare.RightLongitude;
        }
        public bool Contains(MapPoint point)
        {
            return _topLatitude >= point.Latitude && _bottomLatitude <= point.Latitude && _leftLongitude <= point.Longitude && _rightLongitude >= point.Longitude;
        }
    }
}
