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
        //private MapPoint _currentCenter = new MapPoint(59.95, 30.3);
        //private double _currentZoom = 11;

        public IMapService MapService
        {
            get { return _mapService; }
        }

        public MapPoint CurrentCenter
        {
            get { return _mapService.Map.MapCenter; }
            set { _mapService.Map.MapCenter = value; }
        }

        public double CurrentZoom
        {
            get { return _mapService.Map.MapZoom; }
            set { _mapService.Map.MapZoom = value; }
        }

        public MapModel(IMapService mapService)
        {
            _mapService = mapService;
        }
    }
}
