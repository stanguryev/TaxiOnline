using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxiOnline.ClientInfrastructure.Data;
using TaxiOnline.MapEngine.Geometry;

namespace TaxiOnline.MapEngine.Composing
{
    public abstract class BitmapTileSet<TTileCollection, TCollection>
        where TTileCollection : ICollection<MapTile>
        where TCollection : ICollection<BitmapTile>
    {
        protected MapTileSet<TTileCollection> _tileSet;
        protected TCollection _collection;
        private BitmapSize _totalSize;
        private BitmapSize _tileSize;
        private MapPoint _center;
        private double _zoom;

        public BitmapSize TotalSize
        {
            get { return _totalSize; }
        }

        public IEnumerable<BitmapTile> Bitmaps
        {
            get { return _collection; }
        }

        public BitmapTileSet(MapPoint center, double zoom, BitmapSize totalSize, BitmapSize tileSize, MapTileSet<TTileCollection> tileSet)
        {
            _tileSet = tileSet;
            _totalSize = totalSize;
            _tileSize = tileSize;
            _center = center;
            _zoom = zoom;
            _collection = CreateCollection(EnumerateBitmapTiles());
        }

        public void MoveCenter(double newCenterLatitude, double newCenterLongitude)
        {
            _center = new MapPoint(newCenterLatitude, newCenterLongitude);
            _tileSet.MoveCenter(newCenterLatitude, newCenterLongitude);
            if (CheckSettings())
                UpdateTiles();
        }

        public void Zoom(double newZoom)
        {
            _zoom = newZoom;
            _tileSet.Zoom(newZoom);
            if (CheckSettings())
                UpdateTiles();
        }

        public void UpdateTotalSize(BitmapSize size)
        {
            _totalSize = size;
            _collection.Clear();
            foreach (BitmapTile tile in EnumerateBitmapTiles().ToArray())
                _collection.Add(tile);
            if (CheckSettings())
                UpdateTiles();
        }

        public MapPoint? MoveCenterForView(float xOffset, float yOffset)
        {
            BitmapTile tile = _collection.FirstOrDefault() ?? EnumerateBitmapTiles().FirstOrDefault();
            if (tile == null || tile.Bounds.Width * tile.Bounds.Height == 0)
                return null;
            double correctionScale = Math.Pow(2.0, _zoom - tile.Tile.Zoom);
            MapVector size = tile.Tile.GetActualSquare().Size;
            double xScale = size.LongitudeOffset / (double)_tileSize.Width / correctionScale;
            double yScale = size.LatitudeOffset / (double)_tileSize.Height / correctionScale;
            double longitudeOffset = xScale * xOffset;
            double latitudeOffset = yScale * yOffset;
            _center = new MapPoint(_center.Latitude - latitudeOffset, _center.Longitude + longitudeOffset);
            return _center;
        }

        public BitmapSize? GetOffsetFromCoordinates(MapPoint coordinates)
        {
            BitmapTile targetTile = _collection.FirstOrDefault(tile => tile.ContainsCoordinates(coordinates));
            return targetTile == null ? null : (BitmapSize?)targetTile.GetOffsetFromCoordinates(coordinates, _totalSize, _zoom);
        }

        private IEnumerable<BitmapTile> EnumerateBitmapTiles()
        {
            Dictionary<MapTile, BitmapRect> bitmapSquares = _tileSet.Tiles.ToDictionary(tile => tile, tile => GetSquareBitmapBounds(tile));
            if (CheckSettings())
                UpdateTiles(bitmapSquares);
            return bitmapSquares.Select(sq => new BitmapTile(sq.Key, sq.Value));
        }

        protected void UpdateTiles()
        {
            Dictionary<MapTile, BitmapRect> bitmapSquares = _tileSet.Tiles.ToDictionary(tile => tile, tile => GetSquareBitmapBounds(tile));
            UpdateTiles(bitmapSquares);
            //System.Diagnostics.Debug.WriteLine(string.Join(Environment.NewLine, _collection.Select(i => string.Format("{0}, {1}, {2}, {3}", i.Bounds.Left, i.Bounds.Top, i.Bounds.Width, i.Bounds.Height))));
            foreach (BitmapTile tile in _collection.ToArray())
                if (!bitmapSquares.ContainsKey(tile.Tile))
                    _collection.Remove(tile);
            foreach (KeyValuePair<MapTile, BitmapRect> square in bitmapSquares.Where(sq => !_collection.Any(t => sq.Key == t.Tile)).ToArray())
                _collection.Add(new BitmapTile(square.Key, square.Value));
        }

        private void UpdateTiles(Dictionary<MapTile, BitmapRect> bitmapSquares)
        {
            foreach (KeyValuePair<MapTile, BitmapRect> square in bitmapSquares.ToArray())
                if (!CheckBoundsIntersect(square.Value))
                {
                    bitmapSquares.Remove(square.Key);
                    _tileSet.RemoveTile(square.Key);
                }
            if (_tileSet.Tiles.Count() == 0)
                _tileSet.RegenerateTiles();
            MapTile baseTile = _tileSet.Tiles.FirstOrDefault();
            MapTile horizontalAttemptTile = baseTile;
            MapTile verticalAttemptTile;
            while (CheckTileBoundsIntersect(horizontalAttemptTile = horizontalAttemptTile.Offset(1, 0)))
            {
                if (!bitmapSquares.ContainsKey(horizontalAttemptTile))
                {
                    bitmapSquares.Add(horizontalAttemptTile, GetSquareBitmapBounds(horizontalAttemptTile));
                    _tileSet.AddTile(horizontalAttemptTile);
                }
                verticalAttemptTile = horizontalAttemptTile;
                while (CheckTileBoundsIntersect(verticalAttemptTile = verticalAttemptTile.Offset(0, 1)))
                    if (!bitmapSquares.ContainsKey(verticalAttemptTile))
                    {
                        bitmapSquares.Add(verticalAttemptTile, GetSquareBitmapBounds(verticalAttemptTile));
                        _tileSet.AddTile(verticalAttemptTile);
                    }
                verticalAttemptTile = horizontalAttemptTile;
                while (CheckTileBoundsIntersect(verticalAttemptTile = verticalAttemptTile.Offset(0, -1)))
                    if (!bitmapSquares.ContainsKey(verticalAttemptTile))
                    {
                        bitmapSquares.Add(verticalAttemptTile, GetSquareBitmapBounds(verticalAttemptTile));
                        _tileSet.AddTile(verticalAttemptTile);
                    }
            }
            horizontalAttemptTile = baseTile;
            while (CheckTileBoundsIntersect(horizontalAttemptTile = horizontalAttemptTile.Offset(-1, 0)))
            {
                if (!bitmapSquares.ContainsKey(horizontalAttemptTile))
                {
                    bitmapSquares.Add(horizontalAttemptTile, GetSquareBitmapBounds(horizontalAttemptTile));
                    _tileSet.AddTile(horizontalAttemptTile);
                }
                verticalAttemptTile = horizontalAttemptTile;
                while (CheckTileBoundsIntersect(verticalAttemptTile = verticalAttemptTile.Offset(0, 1)))
                    if (!bitmapSquares.ContainsKey(verticalAttemptTile))
                    {
                        bitmapSquares.Add(verticalAttemptTile, GetSquareBitmapBounds(verticalAttemptTile));
                        _tileSet.AddTile(verticalAttemptTile);
                    }
                verticalAttemptTile = horizontalAttemptTile;
                while (CheckTileBoundsIntersect(verticalAttemptTile = verticalAttemptTile.Offset(0, -1)))
                    if (!bitmapSquares.ContainsKey(verticalAttemptTile))
                    {
                        bitmapSquares.Add(verticalAttemptTile, GetSquareBitmapBounds(verticalAttemptTile));
                        _tileSet.AddTile(verticalAttemptTile);
                    }
            }
            verticalAttemptTile = baseTile;
            while (CheckTileBoundsIntersect(verticalAttemptTile = verticalAttemptTile.Offset(0, 1)))
                if (!bitmapSquares.ContainsKey(verticalAttemptTile))
                {
                    bitmapSquares.Add(verticalAttemptTile, GetSquareBitmapBounds(verticalAttemptTile));
                    _tileSet.AddTile(verticalAttemptTile);
                }
            verticalAttemptTile = baseTile;
            while (CheckTileBoundsIntersect(verticalAttemptTile = verticalAttemptTile.Offset(0, -1)))
                if (!bitmapSquares.ContainsKey(verticalAttemptTile))
                {
                    bitmapSquares.Add(verticalAttemptTile, GetSquareBitmapBounds(verticalAttemptTile));
                    _tileSet.AddTile(verticalAttemptTile);
                }
        }

        private BitmapRect GetSquareBitmapBounds(MapTile tile)
        {
            MapSquare square = tile.GetActualSquare();
            double correctionScale = Math.Pow(2.0, _zoom - tile.Zoom);
            MapVector leftTopOffset = new MapVector(square.LeftTop.Latitude - _center.Latitude, square.LeftTop.Longitude - _center.Longitude);
            MapVector squareSize = square.Size;
            double xScale = (double)_tileSize.Width / squareSize.LongitudeOffset * correctionScale;
            int x = (int)Math.Round((double)_totalSize.Width / 2.0 + leftTopOffset.LongitudeOffset * xScale);
            double yTiles = CoordinatesHelper.GetTilesFromLatitude(square.LeftTop.Latitude, tile.Zoom) - CoordinatesHelper.GetTilesFromLatitude(_center.Latitude, tile.Zoom);
            int y = (int)Math.Round((double)_totalSize.Height / 2.0 + (double)_tileSize.Height * yTiles * correctionScale);
            return new BitmapRect(x, y, (int)Math.Ceiling(_tileSize.Width * correctionScale), (int)Math.Ceiling(_tileSize.Height * correctionScale));
        }

        private bool CheckBoundsIntersect(BitmapRect bounds)
        {
            return bounds.Top + bounds.Height >= 0 && bounds.Top <= _totalSize.Height && bounds.Left + bounds.Width >= 0 && bounds.Left <= _totalSize.Width;
        }

        private bool CheckTileBoundsIntersect(MapTile mapTile)
        {
            return CheckBoundsIntersect(GetSquareBitmapBounds(mapTile));
        }

        private bool CheckSettings()
        {
            return _tileSize.Width * _tileSize.Height > 0 && _totalSize.Width * _totalSize.Height > 0;
        }

        protected abstract TCollection CreateCollection(IEnumerable<BitmapTile> enumeration);
    }
}
