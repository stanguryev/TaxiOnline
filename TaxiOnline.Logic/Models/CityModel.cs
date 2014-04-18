using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxiOnline.ClientInfrastructure.Data;

namespace TaxiOnline.Logic.Models
{
    public class CityModel
    {
        private readonly MapModel _map;
        private Guid _id;
        private string _name;
        private MapPoint _initialCenter;
        private double _initialZoom;

        public Guid Id
        {
            get { return _id; }
            internal set { _id = value; }
        }

        public string Name
        {
            get { return _name; }
            internal set { _name = value; }
        }

        public MapPoint InitialCenter
        {
            get { return _initialCenter; }
            internal set { _initialCenter = value; }
        }

        public double InitialZoom
        {
            get { return _initialZoom; }
            internal set { _initialZoom = value; }
        }

        public MapModel Map
        {
            get { return _map; }
        }

        public CityModel(MapModel map)
        {
            _map = map;
        }
    }
}
