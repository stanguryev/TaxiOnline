using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxiOnline.ClientInfrastructure.ServicesEntities.DataService;
using TaxiOnline.ClientServicesAdapter.Data.DataObjects;
using TaxiOnline.ServiceContract.DataContracts;

namespace TaxiOnline.ClientServicesAdapter.Data.ServiceLayer.ServiceLayerObjects
{
    internal class CitySLO : ICityInfo
    {
        private Guid _id;
        private string _name;
        private double _initialLatitude;
        private double _initialLongitude;
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

        public double InitialLatitude
        {
            get { return _initialLatitude; }
            internal set { _initialLatitude = value; }
        }

        public double InitialLongitude
        {
            get { return _initialLongitude; }
            internal set { _initialLongitude = value; }
        }

        public double InitialZoom
        {
            get { return _initialZoom; }
            internal set { _initialZoom = value; }
        }

        public CitySLO(CityDataContract dataContract)
        {
            _id = dataContract.Id;
            _name = dataContract.Name;
            _initialLatitude = dataContract.InitialLatitude;
            _initialLongitude = dataContract.InitialLongitude;
            _initialZoom = dataContract.InitialZoom;
        }

        public CitySLO(CityDTO dto)
        {
            _id = dto.Id;
            _name = dto.CityName;
            _initialLatitude = dto.InitialLatitude;
            _initialLongitude = dto.InitialLongitude;
            _initialZoom = dto.InitialZoom;
        }
    }
}
