using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace TaxiOnline.MapEngine.Geometry
{
    public abstract class MapTileSet<TCollection> where TCollection : ICollection<MapTile>
    {
        protected readonly TCollection _collection;
        private double _zoom;
        private double _centerLatitude;
        private double _centerLongitude;

        public IEnumerable<MapTile> Tiles
        {
            get { return _collection; }
        }

        public MapTileSet(double centerLatitude, double centerLongitude, double zoom)
            : this(new MapSquare(centerLatitude, centerLongitude, zoom), zoom)
        {
            _centerLatitude = centerLatitude;
            _centerLongitude = centerLongitude;
        }

        private MapTileSet(MapSquare square, double zoom)
        {
            _collection = CreateCollection(EnumerateFromBaseAndZoom(square, zoom));
            _zoom = zoom;
        }

        public void MoveCenter(double newCenterLatitude, double newCenterLongitude)
        {
            _centerLatitude = newCenterLatitude;
            _centerLongitude = newCenterLongitude;
            List<MapTile> newTiles = EnumerateFromBaseAndZoom(new MapSquare(newCenterLatitude, newCenterLongitude, _zoom), _zoom).ToList();
            UpdateTiles(newTiles);
        }

        public void Zoom(double newZoom)
        {
            _zoom = newZoom;
            MapSquare baseSquare = new MapSquare(_centerLatitude, _centerLongitude, newZoom);
            MapTile baseTile = new MapTile(baseSquare, newZoom);
            if (_collection.FirstOrDefault().Zoom == MapTile.GetTileZoom(newZoom))
            {
                List<MapTile> newTiles = EnumerateFromBaseAndZoom(new MapSquare(_centerLatitude, _centerLongitude, newZoom), newZoom).ToList();
                UpdateTiles(newTiles);
            }
            else
            {
                _collection.Clear();
                foreach (MapTile tile in EnumerateFromBaseAndZoom(new MapSquare(_centerLatitude, _centerLongitude, newZoom), newZoom).ToArray())
                    _collection.Add(tile);
            }
        }

        public void AddTile(MapTile tile)
        {
            _collection.Add(tile);
        }

        public void RemoveTile(MapTile tile)
        {
            _collection.Remove(tile);
        }

        public void RegenerateTiles()
        {
            foreach (MapTile tile in EnumerateFromBaseAndZoom(new MapSquare(_centerLatitude, _centerLongitude, _zoom), _zoom).ToArray())
                _collection.Add(tile);
        }

        private void UpdateTiles(List<MapTile> newTiles)
        {
            foreach (MapTile oldTile in _collection.Where(tile => !newTiles.Contains(tile)).ToArray())
                _collection.Remove(oldTile);
            newTiles.RemoveAll(tile => newTiles.Contains(tile));
            foreach (MapTile newTile in newTiles)
                _collection.Add(newTile);
        }

        private IEnumerable<MapTile> EnumerateFromBaseAndZoom(MapSquare square, double zoom)
        {
            MapTile baseTile = new MapTile(square, zoom);
            MapSquare baseSquare = baseTile.GetActualSquare();
            int addLeft = 0, addTop = 0, addRight = 0, addBottom = 0;
            MapSquare derivatedSquare = baseSquare;
            while ((derivatedSquare -= baseSquare.LongitudeSize).Intersects(square))
                addLeft++;
            derivatedSquare = baseSquare;
            while ((derivatedSquare += baseSquare.LongitudeSize).Intersects(square))
                addRight++;
            derivatedSquare = baseSquare;
            while ((derivatedSquare -= baseSquare.LatitudeSize).Intersects(square))
                addTop++;
            derivatedSquare = baseSquare;
            while ((derivatedSquare += baseSquare.LatitudeSize).Intersects(square))
                addBottom++;
            yield return baseTile;
            for (int i = -addLeft; i <= addRight; i++)
                for (int j = -addTop; j <= addBottom; j++)
                    if (i != 0 || j != 0)
                        yield return baseTile.Offset(i, j);
        }

        protected abstract TCollection CreateCollection(IEnumerable<MapTile> data);
    }
}
