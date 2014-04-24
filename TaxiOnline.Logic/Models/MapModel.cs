using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TaxiOnline.ClientInfrastructure.Data;
using TaxiOnline.ClientInfrastructure.Services;

namespace TaxiOnline.Logic.Models
{
    public class MapModel
    {
        private readonly IMapService _mapService;
        private MapPoint _currentCenter = new MapPoint(59.95, 30.3);
        private double _currentZoom = 11;

        public IMapService MapService
        {
            get { return _mapService; }
        }

        public MapPoint CurrentCenter
        {
            get { return _currentCenter; }
            set { _currentCenter = value; }
        }

        public double CurrentZoom
        {
            get { return _currentZoom; }
            set { _currentZoom = value; }
        }

        public MapModel(IMapService mapService)
        {
            _mapService = mapService;
        }
    }
}
