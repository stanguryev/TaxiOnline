using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxiOnline.ClientInfrastructure.Data;
using TaxiOnline.MapEngine.Geometry;

namespace TaxiOnline.MapEngine.Composing
{
    public class BitmapTile
    {
        private readonly MapTile _tile;
        private readonly MapSquare _square;
        private readonly BitmapRect _bounds;

        public MapTile Tile
        {
            get { return _tile; }
        }

        public BitmapRect Bounds
        {
            get { return _bounds; }
        }

        public BitmapTile(MapTile tile, BitmapRect bounds)
        {
            _tile = tile;
            _square = tile.GetActualSquare();
            _bounds = bounds;
        }

        public bool ContainsCoordinates(MapPoint coordinates)
        {
            return _square.Contains(coordinates);
        }

        public BitmapSize GetOffsetFromCoordinates(MapPoint coordinates, BitmapSize totalSize, double mapZoom)
        {
            // double correctionScale = Math.Pow(2.0, mapZoom - _tile.Zoom);
            double xScale = (double)_bounds.Width / _square.Size.LongitudeOffset;// *correctionScale;
            MapVector leftTopOffset = new MapVector(_square.LeftTop.Latitude - coordinates.Latitude, coordinates.Longitude - _square.LeftTop.Longitude);
            int x = (int)Math.Round(/*(double)totalSize.Width / 2.0 */ leftTopOffset.LongitudeOffset * xScale);
            double yTiles = CoordinatesHelper.GetTilesFromLatitude(_square.LeftTop.Latitude, _tile.Zoom) - CoordinatesHelper.GetTilesFromLatitude(coordinates.Latitude, _tile.Zoom);
            int y = (int)Math.Round(/*(double)totalSize.Height / 2.0*/ -(double)_bounds.Height * yTiles);// * correctionScale);
            return new BitmapSize(x, y);
        }
    }
}
