using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TaxiOnline.ClientInfrastructure.Data;
using TaxiOnline.ClientInfrastructure.ServicesEntities.DataService;
using TaxiOnline.ServiceContract.DataContracts;

namespace TaxiOnline.ClientServicesAdapter.Data.ServiceLayer.ServiceLayerObjects
{
    internal abstract class AuthenticationRequestSLO : IAuthenticationRequest
    {
        protected readonly Guid _cityId;
        private MapPoint _currentLocation;
        private string _phoneNumber;
        private string _skypeNumber;

        public Guid CityId
        {
            get { return _cityId; }
        }

        public MapPoint CurrentLocation
        {
            get { return _currentLocation; }
            set { _currentLocation = value; }
        }

        public string PhoneNumber
        {
            get { return _phoneNumber; }
            set { _phoneNumber = value; }
        }

        public string SkypeNumber
        {
            get { return _skypeNumber; }
            set { _skypeNumber = value; }
        }

        public AuthenticationRequestSLO(Guid cityId)
        {
            _cityId = cityId;
        }

        public abstract AuthenticationRequestDataContract CreateDataContract();
    }
}
