using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxiOnline.ServerInfrastructure.EntitiesInterfaces;

namespace TaxiOnline.Server.Core.Objects
{
    internal class CityInfo : ICityInfo
    {
        private readonly Guid _id;

        private string _name;
        private double _initialLatitude;
        private double _initialLongitude;
        private double _initialZoom;

        public Guid Id
        {
            get { return _id; }
        }

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }

        public double InitialLatitude
        {
            get { return _initialLatitude; }
            set { _initialLatitude = value; }
        }

        public double InitialLongitude
        {
            get { return _initialLongitude; }
            set { _initialLongitude = value; }
        }

        public double InitialZoom
        {
            get { return _initialZoom; }
            set { _initialZoom = value; }
        }
        
        public CityInfo(Guid id)
        {
            _id = id;
        }
    }
}
